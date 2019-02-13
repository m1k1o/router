using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Protocols
{
    static class ARP
    {
        public static PhysicalAddress RequestTargetMac { get; set; } = PhysicalAddress.Parse("00-00-00-00-00-00");
        public static PhysicalAddress RequestDestinationMac { get; set; } = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

        private static void Send(
            ARPOperation Operation,
            PhysicalAddress SenderMac,
            IPAddress SenderIp,
            PhysicalAddress TargetMac,
            IPAddress TargetIp,
            PhysicalAddress DestionationMac,
            Interface Interface
        )
        {
            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, DestionationMac, EthernetPacketType.Arp);

            var arpPacket = new ARPPacket(Operation, TargetMac, TargetIp, SenderMac, SenderIp);
            ethernetPacket.PayloadData = arpPacket.Bytes;

            Interface.SendPacket(ethernetPacket.Bytes);
        }

        public static void SendRequest(
            IPAddress TargetIp,
            Interface Interface
        )
        {
            Send(
                ARPOperation.Request,
                Interface.PhysicalAddress,
                Interface.IPAddress,
                RequestTargetMac,
                TargetIp,
                RequestDestinationMac,
                Interface
           );
        }

        public static void SendProxyResponse(
            IPAddress SenderIp,
            PhysicalAddress TargetMac,
            IPAddress TargetIp,
            Interface Interface
        )
        {
            Send(
                ARPOperation.Response,
                Interface.PhysicalAddress,
                SenderIp,
                TargetMac,
                TargetIp,
                TargetMac,
                Interface
           );
        }

        public static void SendResponse(
            PhysicalAddress TargetMac,
            IPAddress TargetIp,
            Interface Interface
        )
        {
            Send(
                ARPOperation.Response,
                Interface.PhysicalAddress,
                Interface.IPAddress,
                TargetMac,
                TargetIp,
                TargetMac,
                Interface
           );
        }
    }
}
