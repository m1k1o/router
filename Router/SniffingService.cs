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

        private Dictionary<WebSocket, Action<bool>> Instances = new Dictionary<WebSocket, Action<bool>>();

        private void Start(WebSocket Client, Interface Interface)
        {
            if (Instances.ContainsKey(Client))
            {
                Instances[Client](false);
            }

            void Action(bool Subscribe) {
                var EventHandler = new PacketArrival((Handler Handler) =>
                {
                    var Ethernet = new Ethernet();
                    Ethernet.Import(Handler.EthernetPacket.Bytes);

                    HTTP.WebSockets.Send(Client, "sniffing", Ethernet);
                });

                if (Subscribe)
                {
                    Interface.OnPacketArrival += EventHandler;
                }
                else
                {
                    Interface.OnPacketArrival -= EventHandler;
                }
            };

            Action(true);
            Instances.Add(Client, Action);
        }

        private void Stop(WebSocket Client)
        {
            if (Instances.ContainsKey(Client))
            {
                Instances[Client](false);
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
}
