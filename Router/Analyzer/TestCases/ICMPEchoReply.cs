using PacketDotNet;
using Router.Packets;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Analyzer.TestCases
{
    class ICMPEchoReply : TestCase
    {
        public IPAddress DestinationIP { get; set; }
        public PhysicalAddress DestinationMAC { get; set; }
        
        public override string Default_Name => "ICMP Echo Reply";

        public override string Default_Description =>
            "Test case will send ICMP Echo Request und will be evaluated as succesful after Echo Reply arrival.";
        
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

            if (!Handler.IPv4Packet.ValidChecksum)
            {
                Log("WARNING: ICMP Checksum is NOT valid");
            }
            else
            {
                Log("ICMP Checksum is valid");
            }

            Success();
        }

        protected override void Generate(Interface Interface)
        {
            Log("ICMP Echo Request sent.");

            var ICMP = new ICMP()
            {
                TypeCode = ICMPv4TypeCodes.EchoRequest,
                ID = 1,
                Sequence = 1
            };
            var IP = new IP()
            {
                SourceAddress = Interface.IPAddress,
                DestinationAddress = DestinationIP,
                PayloadPacket = ICMP,
                TimeToLive = 255
            };
            var Ethernet = new Ethernet()
            {
                SourceHwAddress = Interface.PhysicalAddress,
                DestinationHwAddress = DestinationMAC,
                PayloadPacket = IP
            };

            Interface.SendPacket(Ethernet.Export());
        }
    }
}
