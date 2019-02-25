using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    sealed class TCP : GeneratorPayload
    {
        public static IPProtocolType IPProtocolType = IPProtocolType.TCP;

        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }

        public byte Flags { get; set; } = 0;

        public TCP() { }

        public override byte[] Export()
        {
            var TcpPacket = new TcpPacket(SourcePort, DestinationPort)
            {
                AllFlags = Flags
            };

            if (Payload != null)
            {
                TcpPacket.PayloadData = Payload;
            }

            return TcpPacket.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            var TcpPacket = new TcpPacket(new ByteArraySegment(Bytes));

            SourcePort = TcpPacket.SourcePort;
            DestinationPort = TcpPacket.DestinationPort;
            Flags = TcpPacket.AllFlags;
            Payload = TcpPacket.PayloadData;
        }
    }
}
