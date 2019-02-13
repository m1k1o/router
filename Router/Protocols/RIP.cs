using PacketDotNet;
using Router.RIP;
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
            ipPacket.Checksum = ipPacket.CalculateIPChecksum();

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

            if (
                // Interface is not Running RIP
                !Interface.ServiceRunning("rip") ||

                // Sent from me
                Equals(packet.SourceHwAddress, Interface.PhysicalAddress) ||

                (
                    // Not to RIP Multicast MAC
                    !Equals(packet.DestinationHwAddress, MulticastMac) &&

                    // Not to Unicast for me
                    !Equals(packet.DestinationHwAddress, Interface.PhysicalAddress)
                ) ||

                // Not IP Packet
                (ipPacket = (IPv4Packet)packet.Extract(typeof(IPv4Packet))) == null ||

                // Not from this network
                !Interface.IsReachable(ipPacket.SourceAddress) ||

                (
                    // Not to RIP Multicast IP
                    !Equals(ipPacket.DestinationAddress, MulticastIp) &&

                    // Not to me 
                    !Equals(ipPacket.DestinationAddress, Interface.IPAddress)
                ) ||

                // Not UDP Packet
                (udpPacket = (UdpPacket)ipPacket.Extract(typeof(UdpPacket))) == null ||

                // Not to 520 Port
                udpPacket.DestinationPort != PortUDP
            )
            {
                return null;
            }

            return Parse(udpPacket);
        }

        public static RIPPacket Parse(UdpPacket UdpPacket)
        {
            return new RIPPacket(UdpPacket.PayloadData);
        }
    }
}
