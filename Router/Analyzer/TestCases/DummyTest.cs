using Router.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Router.Analyzer.TestCases
{
    class DummyTest : TestCase
    {
        public override string Name => "Dummy Test";

        public override string Description => "Sample test demostrating functionality.";

        public override void Analyze(Handler Handler)
        {
            if (Handler.UdpPacket != null && Handler.UdpPacket.SourcePort == 50 && Handler.UdpPacket.DestinationPort == 50)
            {
                Continue = false;
                Success = true;
            }
        }

        public override void Generate(Interface Interface)
        {
            //var Interface = Interfaces.Instance.GetInterfaceById("1");
            //Interface.SetIP(IPAddress.Parse("192.168.1.5"), IPSubnetMask.Parse("255.255.0.0"));
            //Interface.Start();
            
            var Ethernet_Packet = new Ethernet
            {
                SourceHwAddress = Interface.PhysicalAddress,
                DestinationHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF")
            };
            var IP_Packet = new IP
            {
                SourceAddress = Interface.IPAddress,
                DestinationAddress = IPAddress.Parse("192.168.1.1")
            };
            var UDP_Packet = new UDP
            {
                SourcePort = 50,
                DestinationPort = 50
            };

            // Hierarchy
            Ethernet_Packet.PayloadPacket = IP_Packet;
            IP_Packet.PayloadPacket = UDP_Packet;

            // Send
            Interface.SendPacket(Ethernet_Packet.Export());
        }
    }
}
