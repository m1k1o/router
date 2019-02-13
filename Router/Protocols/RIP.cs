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

        public static bool Validate(Handler H)
        {
            return
                // Interface is Running RIP
                H.Interface.ServiceRunning("rip") &&

                // Sent from me
                !H.IsFromMe &&

                (
                    // To RIP Multicast MAC
                    Equals(H.EthernetPacket.DestinationHwAddress, MulticastMac) ||

                    // To Unicast for me
                    Equals(H.EthernetPacket.DestinationHwAddress, H.Interface.PhysicalAddress)
                ) &&
                
                // Is IPv4
                H.EthernetPacket.Type == EthernetPacketType.IpV4 &&

                // From this network
                H.Interface.IsReachable(H.IPv4Packet.SourceAddress) &&

                (
                    // To RIP Multicast IP
                    Equals(H.IPv4Packet.DestinationAddress, MulticastIp) ||

                    // To Unicast IP for me
                    Equals(H.IPv4Packet.DestinationAddress, H.Interface.IPAddress)
                ) &&

                // Is UDP
                H.IPv4Packet.Protocol == IPProtocolType.UDP &&

                // To 520 Port
                H.UdpPacket.DestinationPort == PortUDP;
        }

        public static RIPPacket Parse(UdpPacket UdpPacket)
        {
            return new RIPPacket(UdpPacket.PayloadData);
        }
    }
}
