using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    sealed class UDP : PacketsImportExport, PacketsPayloadData
    {
        public static IPProtocolType IPProtocolType = IPProtocolType.UDP;

        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }

        public byte[] Payload { get; set; }

        public void PayloadData(byte[] Data) => Payload = Data;

        public UDP() { }

        public byte[] Export()
        {
            var UdpPacket = new UdpPacket(SourcePort, DestinationPort);

            if (Payload != null)
            {
                UdpPacket.PayloadData = Payload;
            }

            return UdpPacket.Bytes;
        }

        public void Import(byte[] Bytes)
        {
            var UdpPacket = new UdpPacket(new ByteArraySegment(Bytes));

            SourcePort = UdpPacket.SourcePort;
            DestinationPort = UdpPacket.DestinationPort;
            Payload = UdpPacket.PayloadData;
        }
    }
}
