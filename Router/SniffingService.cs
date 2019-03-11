using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using Router.Helpers;
using Router.Packets;

namespace Router
{
    class SniffingService : WebSocketService
    {
        object Lock = new object { };

        private Dictionary<WebSocket, SniffingInstance> Instances = new Dictionary<WebSocket, SniffingInstance>();

        private void Start(WebSocket Client, Interface Interface)
        {
            if (Instances.ContainsKey(Client))
            {
                Instances[Client].Unsubscribe();
            }
            else
            {
                Instances.Add(Client, new SniffingInstance(Client));
            }

            Instances[Client].Interface = Interface;
            Instances[Client].Subscribe();
        }

        private void Stop(WebSocket Client)
        {
            if (Instances.ContainsKey(Client))
            {
                Instances[Client].Unsubscribe();
            }
        }

        public void OnConnect(WebSocket Client) { }

        public void OnDisconnect(WebSocket Client)
        {
            Stop(Client);
            Instances.Remove(Client);
        }

        public void OnMessage(WebSocket Client, string Message)
        {
            try
            {
                var Response = JSON.DeserializeAnonymousType(Message, new
                {
                    Key = (string)null,
                    Action = (string)null,
                    Interface = (Interface)null
                });

                if (Response.Key == "sniffing" && Response.Action == "start" && Response.Interface != null)
                {
                    lock (Lock)
                    {
                        Start(Client, Response.Interface);
                    }
                    return;
                }

                if (Response.Key == "sniffing" && Response.Action == "stop")
                {
                    lock (Lock)
                    {
                        Stop(Client);
                    }
                    return;
                }
            }
            catch
            {

            }
        }
    }

    class SniffingInstance
    {
        public Interface Interface { get; set; }
        public WebSocket WebSocket { get; set; }

        public SniffingInstance(WebSocket WebSocket)
        {
            this.WebSocket = WebSocket;
        }

        public void OnPacketArrival(Handler Handler)
        {
            var Ethernet = new Ethernet();
            Ethernet.Import(Handler.EthernetPacket.Bytes);

            HTTP.WebSockets.Send(WebSocket, "sniffing", Ethernet);
        }

        public void Subscribe()
        {
            Interface.OnPacketArrival += OnPacketArrival;
        }

        public void Unsubscribe()
        {
            Interface.OnPacketArrival -= OnPacketArrival;
        }
    }
}
