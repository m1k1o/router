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

            var udpPacket = new UdpPacket(PortUDP, DstPort);
            udpPacket.PayloadData = ripPacket.Bytes;

            var ipPacket = new IPv4Packet(Interface.IPAddress, DstIP);
            ipPacket.PayloadPacket = udpPacket;

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, DstMAC, EthernetPacketType.IpV4);
            ethernetPacket.PayloadPacket = ipPacket;

            Interface.SendPacket(ethernetPacket.Bytes);
        }

        public static void Send(RIPCommandType CommandType, RIPRouteCollection RTEs, Interface Interface)
        {
            Send(MulticastMac, MulticastIp, PortUDP, CommandType, RTEs, Interface);
        }

        public static RIPPacket Parse(EthernetPacket packet, Interface Interface = null)
        {
            // RIP Multicast or Unicast MAC
            if (!Equals(packet.DestinationHwAddress, MulticastMac)/* && !(Interface != null || !Equals(packet.DestinationHwAddress, Interface.PhysicalAddress))*/)
            {
                return null;
            }

            var ipPacket = (IPv4Packet)packet.Extract(typeof(IPv4Packet));
            if (ipPacket == null)
            {
                return null;
            }

            // RIP Multicast or Unicast IP
            if (!Equals(ipPacket.DestinationAddress, MulticastIp)/* && !(Interface != null || !Equals(ipPacket.DestinationAddress, Interface.IPAddress))*/)
            {
                return null;
            }

            var udpPacket = (UdpPacket)packet.Extract(typeof(UdpPacket));
            if (udpPacket == null)
            {
                return null;
            }

            if (udpPacket.DestinationPort != PortUDP)
            {
                return null;
            }

            return new RIPPacket(udpPacket.PayloadData);
        }
    }
}
