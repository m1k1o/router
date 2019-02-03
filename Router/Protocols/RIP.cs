using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;
using Router;
using RIP;
using System.Collections.Generic;

namespace Protocols
{
    class RIP
    {
        public const int PortUDP = 520;
        public const string MulticastIp = "224.0.0.9";
        public const string MulticastMac = "01-00-5E-00-00-09";

        static public void Send(RIPCommandType CommandType, List<RTE> RTEs, Interface Interface)
        {
            var ripPacket = new RIPPacket(CommandType, RTEs);

            var udpPacket = new UdpPacket(PortUDP, PortUDP);
            udpPacket.PayloadData = ripPacket.Export();

            var ipPacket = new IPv4Packet(Interface.IPAddress, IPAddress.Parse(MulticastIp));
            ipPacket.PayloadPacket = udpPacket;

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, PhysicalAddress.Parse(MulticastMac), EthernetPacketType.IpV4);
            ethernetPacket.PayloadPacket = ipPacket;
        }

        static public RIPPacket Parse(EthernetPacket packet, Interface Interface = null)
        {
            // RIP Multicast or Unicast MAC
            if (!Equals(packet.DestinationHwAddress, MulticastMac) && !(Interface == null || !Equals(packet.DestinationHwAddress, Interface.PhysicalAddress)))
            {
                return null;
            }

            var ipPacket = (IPv4Packet)packet.Extract(typeof(IPv4Packet));
            if (ipPacket == null)
            {
                return null;
            }

            // RIP Multicast or Unicast IP
            if (!Equals(ipPacket.DestinationAddress, MulticastIp) && !(Interface == null || !Equals(ipPacket.DestinationAddress, Interface.IPAddress)))
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
