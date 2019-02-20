using PacketDotNet;
using System;
using System.Net;

namespace Router.Generator
{
    abstract class IP : Ethernet
    {
        public IPAddress SourceAddress { get; set; }
        public IPAddress DestinationAddress { get; set; }
        public int TimeToLive { get; set; }

        protected IP() { }

        protected Packet Export(IPProtocolType IPProtocolType, Packet PayloadPacket)
        {
            // Create IP
            var IPv4Packet = new IPv4Packet(SourceAddress, DestinationAddress)
            {
                TimeToLive = TimeToLive,
                Protocol = IPProtocolType,
                PayloadPacket = PayloadPacket
            };
            IPv4Packet.Checksum = IPv4Packet.CalculateIPChecksum();

            // Create Ethernet
            return base.Export(EthernetPacketType.IpV4, IPv4Packet);
        }

        protected new void Parse(string[] Rows, ref int i)
        {
            // Parse Ethernet
            base.Parse(Rows, ref i);

            // Parse IP
            if (Rows.Length - i < 4)
            {
                throw new Exception("Expected SourceAddress, DestinationAddress, TimeToLive.");
            }

            SourceAddress = IPAddress.Parse(Rows[i++]);
            DestinationAddress = IPAddress.Parse(Rows[i++]);
            TimeToLive = Int32.Parse(Rows[i++]);
        }
    }
}
