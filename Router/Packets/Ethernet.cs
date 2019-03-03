using PacketDotNet;
using PacketDotNet.Utils;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    sealed class Ethernet : GeneratorPayload
    {
        public PhysicalAddress SourceHwAddress { get; set; } = PhysicalAddress.Parse("00-00-00-00-00-00");
        public PhysicalAddress DestinationHwAddress { get; set; } = PhysicalAddress.Parse("00-00-00-00-00-00");

        public EthernetPacketType EthernetPacketType { get; set; } = 0;

        public override byte[] Export()
        {
            // Auto Types
            if (PayloadPacket != null && EthernetPacketType == 0)
            {
                if (PayloadPacket is IP)
                {
                    EthernetPacketType = EthernetPacketType.IpV4;
                }
                else if (PayloadPacket is ARP)
                {
                    EthernetPacketType = EthernetPacketType.Arp;
                }
            }

            var EthernetPacket = new EthernetPacket(SourceHwAddress, DestinationHwAddress, EthernetPacketType);
            if (Payload != null)
            {
                EthernetPacket.PayloadData = Payload;
            }

            return EthernetPacket.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            if (Bytes == null) return;

            var EthernetPacket = new EthernetPacket(new ByteArraySegment(Bytes));

            SourceHwAddress = EthernetPacket.SourceHwAddress;
            DestinationHwAddress = EthernetPacket.DestinationHwAddress;
            EthernetPacketType = EthernetPacket.Type;

            // Auto Types
            if (EthernetPacketType == EthernetPacketType.IpV4)
            {
                PayloadPacket = new IP();
                PayloadPacket.Import(EthernetPacket.PayloadPacket.Bytes);
            }
            else if (EthernetPacketType == EthernetPacketType.Arp)
            {
                PayloadPacket = new ARP();
                PayloadPacket.Import(EthernetPacket.PayloadPacket.Bytes);
            }
            else
            {
                // TODO: is PayloadData valid?
                PayloadPacket = new Payload();
                PayloadPacket.Import(EthernetPacket.PayloadData);
            }
        }
    }
}
