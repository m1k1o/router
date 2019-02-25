using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    sealed class ICMP : IGeneratorPacket, IGeneratorPayload
    {
        public static IPProtocolType IPProtocolType = IPProtocolType.ICMP;

        public ICMPv4TypeCodes TypeCode { get; set; }

        public ushort ID { get; set; } = 0;
        public ushort Sequence { get; set; } = 0;

        public byte[] Payload { get; set; }

        public void PayloadData(byte[] Data) => Payload = Data;

        public ICMP() { }

        public byte[] Export()
        {
            var ICMPPacket = new ICMPv4Packet(new ByteArraySegment(new byte[ICMPv4Fields.HeaderLength]))
            {
                TypeCode = TypeCode,
                Checksum = 0,
                ID = ID,
                Sequence = Sequence,
                Data = Payload
            };

            if (Payload != null)
            {
                ICMPPacket.Data = Payload;
            }

            ICMPPacket.Checksum = (ushort)ChecksumUtils.OnesComplementSum(ICMPPacket.Bytes);
            return ICMPPacket.Bytes;
        }

        public void Import(byte[] Bytes)
        {
            var ICMPv4Packet = new ICMPv4Packet(new ByteArraySegment(Bytes));

            TypeCode = ICMPv4Packet.TypeCode;
            ID = ICMPv4Packet.ID;
            Payload = ICMPv4Packet.Data;
        }
    }
}
