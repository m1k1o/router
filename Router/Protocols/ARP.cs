using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;
using Router;

namespace Protocols
{
    class ARP
    {
        public const string RequestTargetMac = "00-00-00-00-00-00";
        public const string RequestDestinationMac = "FF-FF-FF-FF-FF-FF";

        private void Send(
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
            ethernetPacket.PayloadPacket = arpPacket;

            Interface.SendPacket(ethernetPacket.PayloadPacket);
        }

        public void SendRequest(
            IPAddress TargetIp,
            Interface Interface
        )
        {
            Send(
                ARPOperation.Request,
                Interface.PhysicalAddress,
                Interface.IpAddress,
                PhysicalAddress.Parse(RequestTargetMac),
                TargetIp,
                PhysicalAddress.Parse(RequestDestinationMac),
                Interface
           );
        }

        public void SendProxyResponse(
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

        public void SendResponse(
            PhysicalAddress TargetMac,
            IPAddress TargetIp,
            Interface Interface
        )
        {
            Send(
                ARPOperation.Response,
                Interface.PhysicalAddress,
                Interface.IpAddress,
                TargetMac,
                TargetIp,
                TargetMac,
                Interface
           );
        }

        public ARPPacket Parse(Packet packet)
        {
            return (ARPPacket) packet.Extract(typeof(ARPPacket));
        }
    }
}
