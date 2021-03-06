﻿using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Router.Helpers;

namespace Router
{
    static class HTTP
    {
        public static readonly WebSockets WebSockets = new WebSockets();

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
                    WebSockets.Session(context);
                }
                else
                {
                    Request(context);
                }
            }
        }

        private static void Main(string[] args)
        {
            var Preload = Interfaces.Instance;
            Start("http://localhost:7000/");
        }
    }
}
