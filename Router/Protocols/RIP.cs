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

        public void Send(RIPCommandType CommandType, List<RTE> RTEs, Interface Interface)
        {
            var ripPacket = new RIPPacket(CommandType, RTEs);

            var udpPacket = new UdpPacket(PortUDP, PortUDP);
            udpPacket.PayloadData = ripPacket.Export();

            var ipPacket = new IPv4Packet(Interface.GetIp(), IPAddress.Parse(MulticastIp));
            ipPacket.PayloadPacket = udpPacket;

            var ethernetPacket = new EthernetPacket(Interface.GetMac(), PhysicalAddress.Parse(MulticastMac), EthernetPacketType.IpV4);
            ethernetPacket.PayloadPacket = ipPacket;
        }

        public RIPPacket Parse(byte[] Data)
        {
            return new RIPPacket(Data);
        }
    }
}
