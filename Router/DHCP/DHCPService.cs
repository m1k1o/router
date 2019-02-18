using Router.Protocols;
using Router.Protocols.DHCPOptions;
using System;
using System.Net;

namespace Router.DHCP
{
    class DHCPService : InterfaceService
    {
        public string Name { get; } = "dhcp";

        public string Description { get; } = "DHCP";

        public bool OnlyRunningInterface { get; } = true;

        public bool DefaultRunning { get; } = true;

        public bool Anonymous { get; } = true;

        public void OnStarted(Interface Interface) { }

        public void OnStopped(Interface Interface) { }

        public void OnChanged(Interface Interface) { }

        public void OnPacketArrival(Handler Handler)
        {
            if (!Protocols.DHCP.Validate(Handler))
            {
                return;
            }

            var DHCPPacket = new DHCPPacket(Handler.UdpPacket.PayloadData);

            // Server
            if (Handler.UdpPacket.DestinationPort == Protocols.DHCP.ServerPort)
            {
                DHCPServer.OnReceived(Handler.IPv4Packet.DestinationAddress, DHCPPacket, Handler.Interface);
                return;
            }
            /*
            // Client
            if (Handler.UdpPacket.DestinationPort == Protocols.DHCP.ClientPort)
            {
                throw new NotImplementedException();
            }
            */
        }
    }
}
