using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using Router.Helpers;
using Router.Packets;

namespace Router
{
    class SniffingService : WebSocketService
    {
        private Dictionary<WebSocket, Action> Instances = new Dictionary<WebSocket, Action>();

        private void Start(WebSocket Client, Interface Interface)
        {
            void OnPacketArrival(Handler Handler)
            {
                var Ethernet = new Ethernet();
                Ethernet.Import(Handler.EthernetPacket.Bytes);

                HTTP.WebSockets.Send(Client, "sniffing", Ethernet);
            }

            // Subscribe
            Interface.OnPacketArrival += OnPacketArrival;

            // Unsubscribe
            void Unsubscribe() => Interface.OnPacketArrival -= OnPacketArrival;

            Instances.Add(Client, Unsubscribe);
        }

        private void Stop(WebSocket Client)
        {
            if (Instances.ContainsKey(Client))
            {
                Instances[Client]();
                Instances.Remove(Client);
            }
        }

        public void OnConnect(WebSocket Client) { }

        public void OnDisconnect(WebSocket Client)
        {
            Stop(Client);
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
                    Start(Client, Response.Interface);
                    return;
                }

                if (Response.Key == "sniffing" && Response.Action == "stop")
                {
                    Stop(Client);
                    return;
                }
            }
            catch
            {

            }
        }
    }
}
