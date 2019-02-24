using PacketDotNet;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    sealed class Ethernet : PacketsImportExport, PacketsPayloadData
    {
        public PhysicalAddress SourceHwAddress { get; set; }
        public PhysicalAddress DestinationHwAddress { get; set; }

        public EthernetPacketType EthernetPacketType { get; set; }
        public byte[] Payload { get; set; }

        public void PayloadData(byte[] Data) => Payload = Data;

        public byte[] Export()
        {
            var EthernetPacket = new EthernetPacket(SourceHwAddress, DestinationHwAddress, EthernetPacketType);

            if (Payload != null)
            {
                EthernetPacket.PayloadData = Payload;
            }

            return EthernetPacket.Bytes;
        }

        public void Import(byte[] Bytes) {
            var EthernetPacket = new EthernetPacket(new ByteArraySegment(Bytes));

            SourceHwAddress = EthernetPacket.SourceHwAddress;
            DestinationHwAddress = EthernetPacket.DestinationHwAddress;
            EthernetPacketType = EthernetPacket.Type;
            Payload = EthernetPacket.PayloadData;
        }
    }
}
