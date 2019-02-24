using PacketDotNet;
using Router.Helpers;
using System;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    abstract class Ethernet : Generator
    {
        public PhysicalAddress SourceHwAddress { get; set; }
        public PhysicalAddress DestinationHwAddress { get; set; }

        public virtual EthernetPacketType EthernetPacketType { get; set; }
        public virtual byte[] Payload { get; set; }

        protected Ethernet() { }

        public virtual byte[] Export()
        {
            var EthernetPacket = new EthernetPacket(SourceHwAddress, DestinationHwAddress, EthernetPacketType)
            {
                PayloadData = Payload
            };

            return EthernetPacket.Bytes;
        }

        public virtual byte[] ExportAll() => Export();

        /*
        public void Parse(string[] Rows, ref int i)
        {
            if (Rows.Length - i < 3)
            {
                throw new Exception("Expected SourceHwAddress, DestinationHwAddress.");
            }

            SourceHwAddress = Utilities.ParseMAC(Rows[i++]);
            DestinationHwAddress = Utilities.ParseMAC(Rows[i++]);
        }
        */
    }
}
