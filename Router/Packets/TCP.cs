using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    sealed class TCP : GeneratorPayload
    {
        public ushort SourcePort { get; set; } = 0;
        public ushort DestinationPort { get; set; } = 0;

        public ushort Flags { get; set; } = 0;

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
            if (Bytes == null) return;

            var TcpPacket = new TcpPacket(new ByteArraySegment(Bytes));

            SourcePort = TcpPacket.SourcePort;
            DestinationPort = TcpPacket.DestinationPort;
            Flags = TcpPacket.AllFlags;

            PayloadPacket = new Payload();
            PayloadPacket.Import(TcpPacket.PayloadData);
        }
    }
}
