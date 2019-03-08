using System;
using System.Net.WebSockets;
using Router.Helpers;
using Router.Packets;

namespace Router
{
    // TODO: Custom sniffing for each Connection.
    class SniffingService : WebSocketService
    {
        object Lock = new object { };

        private PacketArrival PacketArrival = new PacketArrival((Handler Handler) =>
        {
            var Ethernet = new Ethernet();
            Ethernet.Import(Handler.EthernetPacket.Bytes);

            HTTP.WebSockets.Send("sniffing", Ethernet);
        });

        private Interface ActiveInterface = null;

        private void Start(WebSocket Client, Interface Interface)
        {
            if (ActiveInterface != null)
            {
                ActiveInterface.OnPacketArrival -= PacketArrival;
            }
            
            Interface.OnPacketArrival += PacketArrival;
            ActiveInterface = Interface;
        }

        private void Stop(WebSocket Client)
        {
            if (ActiveInterface != null)
            {
                ActiveInterface.OnPacketArrival -= PacketArrival;
                ActiveInterface = null;
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
