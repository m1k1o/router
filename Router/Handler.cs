using PacketDotNet;
using Router.Protocols;
using SharpPcap;
using System;

namespace Router
{
    class Handler
    {
        public Interface Interface { get; private set; }
        public EthernetPacket EthernetPacket { get; private set; }

        public object PacketPayload { get; private set; }
        public Type PacketType { get; private set; }

        public Handler(RawCapture RawCapture, Interface Interface)
        {
            this.Interface = Interface;

            EthernetPacket = (EthernetPacket)Packet.ParsePacket(RawCapture.LinkLayerType, RawCapture.Data);
            if (EthernetPacket == null)
            {
                throw new Exception("Packet is not Ethernet Packet.");
            }

            // My packet?
            if (Equals(EthernetPacket.SourceHwAddress, Interface.PhysicalAddress))
            {
                PacketType = null;
                return;
            }

            ARPPacket arpPacket = (ARPPacket)EthernetPacket.Extract(typeof(ARPPacket));
            if (arpPacket != null)
            {
                PacketType = typeof(ARPPacket);
                PacketPayload = arpPacket;
                return;
            }

            RIPPacket ripPacket = Protocols.RIP.Parse(EthernetPacket, Interface);
            if (ripPacket != null)
            {
                PacketType = typeof(RIPPacket);
                PacketPayload = ripPacket;
                return;
            }

            IPv4Packet ipPacket = (IPv4Packet)EthernetPacket.Extract(typeof(IPv4Packet));
            if (ipPacket != null)
            {
                PacketType = typeof(IPv4Packet);
                PacketPayload = ipPacket;
                return;
            }

            // Other packets ignore
            PacketType = null;
            return;
        }

        public bool Exists()
        {
            return PacketType != null;
        }

        public bool CheckType(Type Type)
        {
            return Equals(Type, PacketType);
        }

        /*
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
        */
    }
}
