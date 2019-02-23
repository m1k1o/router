using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router
{
    class ICMPEchoReplyService : InterfaceService
    {
        public string Name => "icmp_echo_reply";

        public string Description => "ICMP Echo Reply";

        public bool OnlyRunningInterface => true;

        public bool DefaultRunning => true;

        public bool Anonymous => true;

        public void OnStarted(Interface Interface) { }

        public void OnStopped(Interface Interface) { }

        public void OnChanged(Interface Interface) { }

        public void OnPacketArrival(Handler Handler)
        {
            if (!Handler.CheckEtherType(EthernetPacketType.IpV4))
            {
                return;
            }

            ICMPv4Packet ICMPPacket;
            if (
                // Is not ICMP
                Handler.IPv4Packet.Protocol != IPProtocolType.ICMP ||

                // Is not to my MAC
                !Equals(Handler.EthernetPacket.DestinationHwAddress, Handler.Interface.PhysicalAddress) ||

                // Is not to my IP
                !Equals(Handler.IPv4Packet.DestinationAddress, Handler.Interface.IPAddress) ||
                 
                // Is not valid checksum
                //!Handler.IPv4Packet.ValidChecksum ||

                // Is not ICMP Packet
                (ICMPPacket = (ICMPv4Packet)Handler.InternetLinkLayerPacket.Extract(typeof(ICMPv4Packet))) == null ||

                // Is not EchoRequest
                ICMPPacket.TypeCode != ICMPv4TypeCodes.EchoRequest
            )
            {
                return;
            }

            Console.WriteLine("Got ICMP");
            ICMPPacket.TypeCode = ICMPv4TypeCodes.EchoReply;
            ICMPPacket.Checksum = (ushort)(ICMPPacket.Checksum + ICMPv4TypeCodes.EchoRequest);

            Handler.IPv4Packet.DestinationAddress = Handler.IPv4Packet.SourceAddress;
            Handler.IPv4Packet.SourceAddress = Handler.Interface.IPAddress;
            Handler.IPv4Packet.Checksum = Handler.IPv4Packet.CalculateIPChecksum();

            Handler.EthernetPacket.DestinationHwAddress = Handler.EthernetPacket.SourceHwAddress;
            Handler.EthernetPacket.SourceHwAddress = Handler.Interface.PhysicalAddress;

            Handler.Interface.SendPacket(Handler.EthernetPacket.Bytes);
        }
    }
}
