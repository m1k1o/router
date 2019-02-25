using PacketDotNet;
using PacketDotNet.Utils;
using System.Net;

namespace Router.Packets
{
    sealed class IP : GeneratorPayload
    {
        public IPAddress SourceAddress { get; set; }
        public IPAddress DestinationAddress { get; set; }
        public int TimeToLive { get; set; } = 128;

        public IPProtocolType IPProtocolType { get; set; }

        public IP() { }

        public override byte[] Export()
        {
            var IPv4Packet = new IPv4Packet(SourceAddress, DestinationAddress)
            {
                TimeToLive = TimeToLive,
                Protocol = IPProtocolType
            };

            // Auto Types
            if (PayloadPacket != null && IPProtocolType == IPProtocolType.NONE)
            {
                if (PayloadPacket is ICMP)
                {
                    IPProtocolType = IPProtocolType.ICMP;
                }
                else if (PayloadPacket is UDP)
                {
                    IPProtocolType = IPProtocolType.UDP;
                }
                else if (PayloadPacket is TCP)
                {
                    IPProtocolType = IPProtocolType.TCP;
                }
            }

            if (Payload != null)
            {
                IPv4Packet.PayloadData = Payload;
                IPv4Packet.PayloadLength = (ushort)Payload.Length;
            }

            IPv4Packet.Checksum = IPv4Packet.CalculateIPChecksum();
            return IPv4Packet.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            var IPv4Packet = new IPv4Packet(new ByteArraySegment(Bytes));

            SourceAddress = IPv4Packet.SourceAddress;
            DestinationAddress = IPv4Packet.DestinationAddress;
            TimeToLive = IPv4Packet.TimeToLive;
            IPProtocolType = IPv4Packet.Protocol;

            // Auto Types
            if (IPProtocolType == IPProtocolType.ICMP)
            {
                PayloadPacket = new ICMP();
                PayloadPacket.Import(IPv4Packet.PayloadData);
            }
            else if (IPProtocolType == IPProtocolType.UDP)
            {
                PayloadPacket = new UDP();
                PayloadPacket.Import(IPv4Packet.PayloadData);
            }
            else if (IPProtocolType == IPProtocolType.TCP)
            {
                PayloadPacket = new TCP();
                PayloadPacket.Import(IPv4Packet.PayloadData);
            }
            else
            {
                Payload = IPv4Packet.PayloadData;
            }
        }
    }
}
