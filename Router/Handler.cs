using PacketDotNet;
using Router.Protocols;
using Router.RIP;
using Router.ARP;
using SharpPcap;
using System;

namespace Router
{
    class Handler
    {
        private Interface Interface;
        private EthernetPacket EthernetPacket;

        private Type PacketType;
        private Action PacketHandler;

        public Handler(RawCapture RawCapture, Interface Interface)
        {
            this.Interface = Interface;

            var packet = (EthernetPacket)PacketDotNet.Packet.ParsePacket(RawCapture.LinkLayerType, RawCapture.Data);
            EthernetPacket = packet;

            if (packet == null)
            {
                throw new Exception("Packet is not Ethernet Packet.");
            }

            // My packet?
            if (Equals(packet.SourceHwAddress, Interface.PhysicalAddress))
            {
                PacketType = null;
                return;
            }

            ARPPacket arpPacket = (ARPPacket)packet.Extract(typeof(ARPPacket));
            if (arpPacket != null)
            {
                PacketType = typeof(ARPPacket);
                PacketHandler = () => ARP(arpPacket);
                return;
            }

            RIPPacket ripPacket = Protocols.RIP.Parse(packet, Interface);
            if (ripPacket != null)
            {
                PacketType = typeof(RIPPacket);
                PacketHandler = () => RIP(ripPacket);
                return;
            }

            IPv4Packet ipPacket = (IPv4Packet)packet.Extract(typeof(IPv4Packet));
            if (ipPacket != null)
            {
                PacketType = typeof(IPv4Packet);
                PacketHandler = () => IP(ipPacket);
                return;
            }

            // Other packets ignore
            PacketType = null;
            return;
        }

        public bool Exists()
        {
            return PacketHandler != null;
        }

        public bool CheckType(Type Type)
        {
            return Equals(Type, PacketType);
        }

        public void Execute()
        {
            if (PacketType != null)
            {
                PacketHandler();
            }
        }

        public void RIP(RIPPacket RIPPacket)
        {
            Console.WriteLine("Got RIP.");
            IPv4Packet IPPacket = (IPv4Packet)EthernetPacket.Extract(typeof(IPv4Packet));

            if (RIPPacket.CommandType == RIPCommandType.Request)
            {
                UdpPacket UDPPacket = (UdpPacket)IPPacket.Extract(typeof(UdpPacket));
                RIPRequest.OnReceived(EthernetPacket.SourceHwAddress, IPPacket.SourceAddress, UDPPacket.SourcePort, RIPPacket.RouteCollection, Interface);
                return;
            }

            if (RIPPacket.CommandType == RIPCommandType.Response)
            {
                RIPResponse.OnReceived(IPPacket.SourceAddress, RIPPacket.RouteCollection, Interface);
                return;
            }
        }

        public void ARP(ARPPacket ARPPacket)
        {
            Console.WriteLine("Got ARP.");
            ARPMiddleware.OnReceived(EthernetPacket.DestinationHwAddress, ARPPacket, Interface);
        }

        public void IP(IPv4Packet IPv4Packet)
        {
            Console.WriteLine("Got IPV4.");
            if (
                // Is from my MAC
                Equals(EthernetPacket.SourceHwAddress, Interface.PhysicalAddress) || 

                // Is not to my MAC
                !Equals(EthernetPacket.DestinationHwAddress, Interface.PhysicalAddress) ||

                // Is to my IP
                Equals(IPv4Packet.DestinationAddress, Interface.IPAddress) ||

                // Is to my Device IP
                (Interface.DeviceIP != null && Equals(IPv4Packet.DestinationAddress, Interface.DeviceIP))
            )
            {
                return;
            }

            Console.WriteLine("Routing to {0}.", IPv4Packet.DestinationAddress);
            Routing.OnReceived(IPv4Packet);
        }
    }
}
