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

            if(Handler.UdpPacket.DestinationPort != Protocols.DHCP.ServerPort)
            {
                Console.WriteLine("Accepting only DHCP Server Port datagrams.");
                return;
            }

            // Proccess DHCP
            var DHCPPacket = new DHCPPacket(Handler.UdpPacket.PayloadData);
            Console.WriteLine("Got DHCP Packet with OperationCode: " + DHCPPacket.OperationCode.ToString());
            Console.WriteLine("Got DHCP Packet with MAC: " + DHCPPacket.ClientMACAddress.ToString());
            var Options = DHCPPacket.Options;
            Console.WriteLine("Got DHCP Packet MessageType: " + DHCPPacket.Options.MessageType);

            /*
            if (DHCPPacket.Options.MessageType == DHCPMessageType.Discover)
            {
                Protocols.DHCP.SendOffer(DHCPPacket.TransactionID, DHCPPacket.ClientMACAddress, IPAddress.Parse("192.168.1.2"), Handler.Interface);
                return;
            }

            if (DHCPPacket.Options.MessageType == DHCPMessageType.Request)
            {
                Protocols.DHCP.SendACK(DHCPPacket.TransactionID, DHCPPacket.ClientMACAddress, IPAddress.Parse("192.168.1.2"), Handler.Interface);
                return;
            }
            */

            /*
            foreach (var Option in Options)
            {
                if (Option is DHCPUnknownOption)
                {
                    Console.WriteLine("\t^^ unknown.");
                }
                else
                {
                    Console.WriteLine("\t^^ Option: " + Option.ToString());
                }
            }
            */
        }
    }
}
