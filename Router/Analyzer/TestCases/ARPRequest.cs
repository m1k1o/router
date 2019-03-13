using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Analyzer.TestCases
{
    class ARPRequest : TestCase
    {
        static PhysicalAddress BroadcastMAC = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");
        static PhysicalAddress ZeroMAC = PhysicalAddress.Parse("00-00-00-00-00-00");

        public PhysicalAddress DeviceMAC { get; set; } = null;
        public IPAddress ExpectedIP { get; set; } = null;

        public override string Default_Name => "ARP Request";

        public override string Default_Description =>
            "Test case will wait for ARP Request from expected device.";

        protected override void Analyze(Handler Handler)
        {
            // Is ARP and relevant for me?
            if (
                !Handler.CheckEtherType(EthernetPacketType.Arp) &&
                !Equals(Handler.EthernetPacket.DestinationHwAddress, Handler.Interface.PhysicalAddress) &&
                !Equals(Handler.EthernetPacket.DestinationHwAddress, BroadcastMAC)
            )
            {
                return;
            }

            var ARPPacket = (ARPPacket)Handler.InternetLinkLayerPacket.PayloadPacket;

            if (!Equals(ARPPacket.TargetHardwareAddress, ZeroMAC))
            {
                Log("Received ARP. Target MAC must be 00:00:00:00:00:00.");
                return;
            }

            Log(
                "Received ARP:\n" +
                "\tSender MAC: " + ARPPacket.SenderHardwareAddress + "\n" +
                "\tTarget IP: " + ARPPacket.TargetProtocolAddress + "\n"
            );

            // Any ARP Request is success
            if (DeviceMAC == null && ExpectedIP == null)
            {
                Success();
                return;
            }

            // ARP Request asking ExpectedIP is success
            if (DeviceMAC == null)
            {
                if (Equals(ARPPacket.TargetProtocolAddress, ExpectedIP))
                {
                    Success();
                }

                return;
            }

            // ARP Request from DeviceMAC is success
            if (ExpectedIP == null)
            {
                if (Equals(ARPPacket.SenderHardwareAddress, DeviceMAC) && Equals(Handler.EthernetPacket.SourceHwAddress, DeviceMAC))
                {
                    Success();
                }

                return;
            }

            // Only ARP Request from DeviceMAC can be evaluated
            if (Equals(ARPPacket.SenderHardwareAddress, DeviceMAC) && Equals(Handler.EthernetPacket.SourceHwAddress, DeviceMAC))
            {
                // Only asking ExpectedIP is success
                if (Equals(ARPPacket.TargetProtocolAddress, ExpectedIP))
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
            var Str = "Waiting for ARP Request.";

            if(DeviceMAC != null)
            {
                Str += "\n\tFrom: " + DeviceMAC;
            }

            if (ExpectedIP != null)
            {
                Str += "\n\tExpected IP: " + ExpectedIP;
            }

            Log(Str);
        }
    }
}
