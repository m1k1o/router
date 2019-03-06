using PacketDotNet;
using Router.Packets;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Analyzer.TestCases
{
    class EchoReply : TestCase
    {
        public IPAddress IP { get; set; }
        public PhysicalAddress MAC { get; set; }
        
        public override string Name => "Echo Reply";

        public override string Description => "Testing ICMP Echo Reply.";
        
        protected override void Analyze(Handler Handler)
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

                // Is not EchoReply
                ICMPPacket.TypeCode != ICMPv4TypeCodes.EchoReply
            )
            {
                return;
            }

            Log("Got ICMP Reply.");
            Success();
        }

        protected override void Generate(Interface Interface)
        {
            var ICMP = new ICMP()
            {
                TypeCode = ICMPv4TypeCodes.EchoRequest,
                ID = 1,
                Sequence = 1
            };
            var IP = new IP()
            {
                SourceAddress = Interface.IPAddress,
                DestinationAddress = this.IP,
                PayloadPacket = ICMP
            };
            var Ethernet = new Ethernet()
            {
                SourceHwAddress = Interface.PhysicalAddress,
                DestinationHwAddress = MAC,
                PayloadPacket = IP
            };

            Interface.SendPacket(Ethernet.Export());
        }
    }
}
