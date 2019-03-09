using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Analyzer.TestCases
{
    class ARPRequest : TestCase
    {
        public IPAddress RequestedIP { get; set; }
        public PhysicalAddress ExpectedMAC { get; set; } = null;

        public override string Default_Name => "ARP Request";

        public override string Default_Description => "Testing ARP.";

        protected override void Analyze(Handler Handler)
        {
            if (!Handler.CheckEtherType(EthernetPacketType.Arp))
            {
                return;
            }

            Log("Received ARP.");
            var ARPPacket = (ARPPacket)Handler.InternetLinkLayerPacket.PayloadPacket;

            // Is packet valid response for me?
            if (
                Equals(Handler.EthernetPacket.DestinationHwAddress, Handler.Interface.PhysicalAddress) &&
                Equals(ARPPacket.TargetHardwareAddress, Handler.Interface.PhysicalAddress) &&
                Equals(ARPPacket.TargetProtocolAddress, Handler.Interface.IPAddress)
            )
            {
                Log(
                    "ARP Response is for me:\n" +
                    "\tSender IP: " + ARPPacket.SenderProtocolAddress + "\n" +
                    "\tSender MAC: " + ARPPacket.SenderHardwareAddress
                );

                if (ExpectedMAC == null && Equals(ARPPacket.SenderProtocolAddress, RequestedIP))
                {
                    Success();
                    return;
                }

                if (
                    Equals(ARPPacket.SenderProtocolAddress, RequestedIP) &&
                    Equals(ARPPacket.SenderHardwareAddress, ExpectedMAC)
                )
                {
                    Success();
                }

                if (
                    (Equals(ARPPacket.SenderProtocolAddress, RequestedIP) && !Equals(ARPPacket.SenderHardwareAddress, ExpectedMAC)) ||
                    (!Equals(ARPPacket.SenderProtocolAddress, RequestedIP) && Equals(ARPPacket.SenderHardwareAddress, ExpectedMAC))
                )
                {
                    Error();
                }
            }
        }

        protected override void Generate(Interface Interface)
        {
            Protocols.ARP.SendRequest(RequestedIP, Interface);
            if (ExpectedMAC == null)
            {
                Log("Who has " + RequestedIP + "? Tell me.");
            }
            else
            {
                Log("Who has " + RequestedIP + "? " + ExpectedMAC + " should.");
            }
        }
    }
}
