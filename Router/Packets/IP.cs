using PacketDotNet;
using PacketDotNet.Utils;
using System.Net;

namespace Router.Packets
{
    sealed class IP : GeneratorPayload
    {
        public IPAddress SourceAddress { get; set; } = IPAddress.Parse("0.0.0.0");
        public IPAddress DestinationAddress { get; set; } = IPAddress.Parse("0.0.0.0");
        public int TimeToLive { get; set; } = 0;

        public IPProtocolType IPProtocolType { get; set; } = 0;

        public IP() { }

        public override byte[] Export()
        {
            // Auto Types
            if (PayloadPacket != null && IPProtocolType == 0)
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

            var IPv4Packet = new IPv4Packet(SourceAddress, DestinationAddress)
            {
                TimeToLive = TimeToLive,
                Protocol = IPProtocolType
            };

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
            if (Bytes == null) return;

            var IPv4Packet = new IPv4Packet(new ByteArraySegment(Bytes));

            SourceAddress = IPv4Packet.SourceAddress;
            DestinationAddress = IPv4Packet.DestinationAddress;
            TimeToLive = IPv4Packet.TimeToLive;
            IPProtocolType = IPv4Packet.Protocol;

            // Auto Types
            if (IPProtocolType == IPProtocolType.ICMP)
            {
                PayloadPacket = new ICMP();
                PayloadPacket.Import(IPv4Packet.PayloadPacket.Bytes);
            }
            else if (IPProtocolType == IPProtocolType.UDP)
            {
                PayloadPacket = new UDP();
                PayloadPacket.Import(IPv4Packet.PayloadPacket.Bytes);
            }
            else if (IPProtocolType == IPProtocolType.TCP)
            {
                PayloadPacket = new TCP();
                PayloadPacket.Import(IPv4Packet.PayloadPacket.Bytes);
            }
            else
            {
                // TODO: is PayloadData valid?
                PayloadPacket = new Payload();
                PayloadPacket.Import(IPv4Packet.PayloadData);
            }
        }
    }
}
