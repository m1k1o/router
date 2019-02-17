using System;

namespace Router.LLDP
{
    class DHCPServerService : InterfaceService
    {
        public string Name { get; } = "dhcp_server";

        public string Description { get; } = "DHCP Server";

        public bool OnlyRunningInterface { get; } = true;

        public bool DefaultRunning { get; } = false;

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

            if(Handler.UdpPacket.DestinationPort != Protocols.DHCP.ServerPort)
            {
                Console.WriteLine("Accepting only DHCP Server Port datagrams.");
                return;
            }
            
            // Proccess DHCP
        }
    }
}
