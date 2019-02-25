using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    sealed class ICMP : GeneratorPayload
    {
        public ICMPv4TypeCodes TypeCode { get; set; }

        public ushort ID { get; set; } = 0;
        public ushort Sequence { get; set; } = 0;

        public ICMP() { }

        public override byte[] Export()
        {
            var ICMPPacket = new ICMPv4Packet(new ByteArraySegment(new byte[ICMPv4Fields.HeaderLength]))
            {
                TypeCode = TypeCode,
                Checksum = 0,
                ID = ID,
                Sequence = Sequence
            };

            if (Payload != null)
            {
                ICMPPacket.Data = Payload;
            }

            ICMPPacket.Checksum = (ushort)ChecksumUtils.OnesComplementSum(ICMPPacket.Bytes);
            return ICMPPacket.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            var ICMPv4Packet = new ICMPv4Packet(new ByteArraySegment(Bytes));

            TypeCode = ICMPv4Packet.TypeCode;
            ID = ICMPv4Packet.ID;

            // Some Types contains IP Header
            var Type = (byte)((ushort)TypeCode >> 8);
            switch (Type)
            {
                case 3:
                case 5:
                case 11:
                case 12:
                    PayloadPacket = new IP();
                    PayloadPacket.Import(ICMPv4Packet.Data);
                    break;
                default:
                    Payload = ICMPv4Packet.Data;
                    break;
            }
        }
    }
}
