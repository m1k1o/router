using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Router.Helpers;

namespace Router
{
    static class HTTP
    {
        private static void Request(HttpListenerContext context)
        {
            var Request = context.Request;
            string Data;
            using (var Reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            {
                Data = Reader.ReadToEnd();
            }

            if (context.Response.OutputStream.CanWrite)
            {
                string ResponseString = Response(context.Request.RawUrl, Data);
                byte[] ResponseBytes = Encoding.UTF8.GetBytes(ResponseString);

                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.OutputStream.Write(ResponseBytes, 0, ResponseBytes.Length);
                context.Response.KeepAlive = false;
            }

            try
            {
                context.Response.Close();
            }
            catch { };
        }

        private static string Response(string URL, string Data)
        {
            // Process Request
            String[] Args = URL.Split('/');

            if (Args.Length != 3)
            {
                return "<html><head><title>Router API</title></head>" +
                           "<body>Welcome to <strong>Router API</strong><br>Request URL: " + URL + "</em></body>" +
                       "</html>";
            }

            String NamespaceName = Args[1];
            String ClassName = Args[2];

            try
            {
                Type Type = Type.GetType("Router.Controllers." + NamespaceName + "." + ClassName);
                if (Type == null)
                {
                    throw new Exception("Endpoint'" + NamespaceName + "." + ClassName + "' not found.");
                }

                ConstructorInfo Constructor = Type.GetConstructor(Type.EmptyTypes);
                var Controller = (Controllers.Controller)Constructor.Invoke(Type.EmptyTypes);

                // Populate
                if (!string.IsNullOrEmpty(Data))
                {
                    JSON.PopulateObject(Data, Controller);
                }

                // Execute
                if (Controller is Controllers.Executable)
                {
                    ((Controllers.Executable)Controller).Execute();
                }

                // Export
                return JSON.SerializeObject(Controller.Export());
            }
            catch (Exception e)
            {
                return JSON.SerializeObject(JSON.Error(e.Message));
            }
        }
        
        private static void Start(String URL)
        {
            var httpListener = new HttpListener();
            Console.WriteLine("Starting server...");
            httpListener.Prefixes.Add(URL);
            httpListener.Start();
            Console.WriteLine("Server started.");

            while (true)
            {
                HttpListenerContext context = httpListener.GetContext();

                if (context.Request.IsWebSocketRequest)
                {
                    WebSocket(context);
                }
                else
                {
                    Request(context);
                }
            }
        }

        private static List<WebSocket> WebSocketClients = new List<WebSocket>();

        public static void WebSocketSend(string Key, object Data)
        {
            var DataString = JSON.SerializeObject(new
            {
                key = Key,
                data = Data
            });

            var DataBytes = Encoding.UTF8.GetBytes(DataString);
            var DataSegment = new ArraySegment<byte>(DataBytes);

            var Clients = WebSocketClients.ToArray();
            foreach (var Client in Clients)
            {
                Client.SendAsync(DataSegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private static async void WebSocket(HttpListenerContext context)
        {
            Console.WriteLine("New WS Session.");
            var ws = (await context.AcceptWebSocketAsync(subProtocol: null)).WebSocket;
            WebSocketClients.Add(ws);

            while (ws.State == WebSocketState.Open)
            {
                try
                {
                    var buf = new ArraySegment<byte>(new byte[1024]);
                    var ret = await ws.ReceiveAsync(buf, CancellationToken.None);

                    if (ret.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("WS Session Close.");
                        break;
                    }

                    Console.WriteLine("WS Got Message.");
                }
                catch
                {
                    break;
                }
            }

            WebSocketClients.Remove(ws);
            ws.Dispose();
        }

        private static void Main(string[] args)
        {
            var Preload = Interfaces.Instance;
            Start("http://localhost:7000/");
        }
    }
}
