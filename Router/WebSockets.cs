using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Router
{

    delegate void WebSocketEvent(WebSocket Client);
    delegate void WebSocketMessage(WebSocket Client, string Message);

    class WebSockets
    {
        private static List<WebSocketService> Services = new List<WebSocketService>
        {
            new SniffingService()
        };

        public event WebSocketEvent OnConnect = delegate { };
        public event WebSocketEvent OnDisconnect = delegate { };
        public event WebSocketMessage OnMessage = delegate { };

        private List<WebSocket> WebSocketClients = new List<WebSocket>();

        public WebSockets()
        {
            ServicesInitialize();
        }

        private ArraySegment<byte> MessageToSegment(string Key, object Data)
        {
            var DataString = JSON.SerializeObject(new
            {
                key = Key,
                data = Data
            });

            var DataBytes = Encoding.UTF8.GetBytes(DataString);
            return new ArraySegment<byte>(DataBytes);
        }

        public void Send(WebSocket Client, string Key, object Data)
        {
            Client.SendAsync(MessageToSegment(Key, Data), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void Send(string Key, object Data)
        {
            var DataSegment = MessageToSegment(Key, Data);

            var Clients = WebSocketClients.ToArray();
            foreach (var Client in Clients)
            {
                Client.SendAsync(DataSegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async void Session(HttpListenerContext context)
        {
            Console.WriteLine("New WS Session.");
            var ws = (await context.AcceptWebSocketAsync(subProtocol: null)).WebSocket;
            OnConnect(ws);
            WebSocketClients.Add(ws);

            while (ws.State == WebSocketState.Open)
            {
                try
                {
                    var ReceiveBuffer = new byte[1024];
                    var Response = await ws.ReceiveAsync(new ArraySegment<byte>(ReceiveBuffer), CancellationToken.None);

                    if (Response.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("WS Session Close.");
                        break;
                    }

                    Console.WriteLine("WS Got Message.");
                    OnMessage(ws, Encoding.ASCII.GetString(ReceiveBuffer));
                }
                catch
                {
                    break;
                }
            }

            WebSocketClients.Remove(ws);
            OnDisconnect(ws);
            ws.Dispose();
        }

        private void ServicesInitialize()
        {
            foreach (var Service in Services)
            {
                OnConnect += Service.OnConnect;
                OnDisconnect += Service.OnDisconnect;
                OnMessage += Service.OnMessage;
            }
        }
    }

    interface WebSocketService
    {
        void OnConnect(WebSocket Client);

        void OnDisconnect(WebSocket Client);

        void OnMessage(WebSocket Client, string Message);
    }
}
