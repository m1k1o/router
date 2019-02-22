using PacketDotNet;
using Router.Helpers;
using System;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    abstract class Ethernet
    {
        public PhysicalAddress SourceHwAddress { get; set; }
        public PhysicalAddress DestinationHwAddress { get; set; }

        protected Ethernet() { }

        protected PacketDotNet.Packet Export(EthernetPacketType EthernetPacketType, PacketDotNet.Packet PayloadPacket)
        {
            var EthernetPacket = new EthernetPacket(SourceHwAddress, DestinationHwAddress, EthernetPacketType)
            {
                PayloadPacket = PayloadPacket
            };

            return EthernetPacket;
        }

        protected void Parse(string[] Rows, ref int i)
        {
            if (Rows.Length - i < 3)
            {
                throw new Exception("Expected SourceHwAddress, DestinationHwAddress.");
            }

            SourceHwAddress = Utilities.ParseMAC(Rows[i++]);
            DestinationHwAddress = Utilities.ParseMAC(Rows[i++]);
        }
    }
}
