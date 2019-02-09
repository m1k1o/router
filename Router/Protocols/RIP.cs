using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Protocols
{
    static class RIP
    {
        public static ushort PortUDP { get; set; } = 520;
        public static IPAddress MulticastIp { get; set; } = IPAddress.Parse("224.0.0.9");
        public static PhysicalAddress MulticastMac { get; set; } = PhysicalAddress.Parse("01-00-5E-00-00-09");

        public static void Send(PhysicalAddress DstMAC, IPAddress DstIP, ushort DstPort, RIPCommandType CommandType, RIPRouteCollection RTEs, Interface Interface)
        {
            var ripPacket = new RIPPacket(CommandType, RTEs);

            var udpPacket = new UdpPacket(PortUDP, DstPort)
            {
                PayloadData = ripPacket.Bytes
            };

            var ipPacket = new IPv4Packet(Interface.IPAddress, DstIP)
            {
                PayloadPacket = udpPacket
            };

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, DstMAC, EthernetPacketType.IpV4)
            {
                PayloadPacket = ipPacket
            };

            Interface.SendPacket(ethernetPacket.Bytes);
        }

        public static void Send(RIPCommandType CommandType, RIPRouteCollection RTEs, Interface Interface)
        {
            Send(MulticastMac, MulticastIp, PortUDP, CommandType, RTEs, Interface);
        }

        public static RIPPacket Parse(EthernetPacket packet, Interface Interface)
        {
            IPv4Packet ipPacket;
            UdpPacket udpPacket;

            if(
                // Sent from me
                Equals(packet.SourceHwAddress, Interface.PhysicalAddress) &&

                // Not from RIP Multicast MAC
                !Equals(packet.DestinationHwAddress, MulticastMac) &&

                // Not from Unicast for me
                !Equals(packet.DestinationHwAddress, MulticastMac) &&

                // Not IP Packet
                (ipPacket = (IPv4Packet)packet.Extract(typeof(IPv4Packet))) == null &&

                // Not from this network
                !Interface.IsReachable(Interface.IPAddress) &&

                // Not from RIP Multicast IP
                !Equals(ipPacket.DestinationAddress, MulticastIp) &&

                // Not for me 
                !Equals(ipPacket.DestinationAddress, Interface.IPAddress) &&

                // Not UDP Packet
                (udpPacket = (UdpPacket)ipPacket.Extract(typeof(UdpPacket))) == null &&

                // Not to 520 Port
                udpPacket.DestinationPort != PortUDP
            )
            {
                return Parse(udpPacket);
            }

            return null;
        }

        public static RIPPacket Parse(UdpPacket UdpPacket)
        {
            return new RIPPacket(UdpPacket.PayloadData);
        }
    }
}
