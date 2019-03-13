using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Analyzer.TestCases
{
    class ICMPEchoRequest : TestCase
    {
        public PhysicalAddress SourceMAC { get; set; } = null;
        public IPAddress SourceIP { get; set; } = null;

        public override string Default_Name => "ICMP Echo Request";

        public override string Default_Description =>
            "Test case will wait for ICMP Echo Request from expected device.";
        
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
                ICMPPacket.TypeCode != ICMPv4TypeCodes.EchoRequest
            )
            {
                return;
            }

            Log("Got ICMP Request.");

            if (!Handler.IPv4Packet.ValidChecksum)
            {
                Log("WARNING: ICMP Checksum is NOT valid");
            }
            else
            {
                Log("ICMP Checksum is valid");
            }

            // Any ICMP Echo Request is success
            if (SourceMAC == null && SourceIP== null)
            {
                Success();
                return;
            }

            // ICMP Echo Request sent from Source MAC ist successful
            if (SourceMAC == null)
            {
                if (Equals(Handler.EthernetPacket.SourceHwAddress, SourceMAC))
                {
                    Success();
                }

                return;
            }

            // ICMP Echo Request sent from Source IP ist successful
            if (SourceIP == null)
            {
                if (Equals(Handler.IPv4Packet.SourceAddress, SourceIP))
                {
                    Success();
                }

                return;
            }

            // Only ARP Request from Source MAC can be evaluated
            if (Equals(Handler.EthernetPacket.SourceHwAddress, SourceMAC))
            {
                // Only right SourceIP is success
                if (Equals(Handler.IPv4Packet.SourceAddress, SourceIP))
                {
                    Success();
                }
                else
                {
                    Error();
                }
            }
        }

        protected override void Generate(Interface Interface)
        {
            var Str = "Waiting for ICMP Echo Request.";
            Str += "\n\tDestination MAC: " + Interface.PhysicalAddress;
            Str += "\n\tDestination IP: " + Interface.IPAddress;

            if (SourceMAC != null)
            {
                Str += "\n\tSource MAC: " + SourceMAC;
            }

            if (SourceIP != null)
            {
                Str += "\n\tSource IP: " + SourceIP;
            }

            Log(Str);
        }
    }
}
