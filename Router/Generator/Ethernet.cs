using PacketDotNet;
using System;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    abstract class Ethernet
    {
        public PhysicalAddress SourceHwAddress { get; set; }
        public PhysicalAddress DestinationHwAddress { get; set; }

        protected Ethernet() { }

        protected Packet Export(EthernetPacketType EthernetPacketType, Packet PayloadPacket)
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

            SourceHwAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            DestinationHwAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
        }
    }
}
