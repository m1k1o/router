using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Protocols
{
    static class ARP
    {
        static public PhysicalAddress RequestTargetMac { get; set; } = PhysicalAddress.Parse("00-00-00-00-00-00");
        static public PhysicalAddress RequestDestinationMac { get; set; } = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

        static private void Send(
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

        static public void SendRequest(
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

        static public void SendProxyResponse(
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

        static public void SendResponse(
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

        static public ARPPacket Parse(Packet packet)
        {
            return (ARPPacket) packet.Extract(typeof(ARPPacket));
        }
    }
}
