using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    class ICMP : IP, Generator
    {
        public ICMPv4TypeCodes TypeCode { get; set; }

        public ushort ID { get; set; } = 0;
        public ushort Sequence { get; set; } = 0;

        public byte[] Data { get; set; }

        public ICMP() { }

        public new byte[] Export()
        {
            var ICMPPacket = new ICMPv4Packet(new ByteArraySegment(new byte[ICMPv4Fields.HeaderLength]))
            {
                TypeCode = TypeCode,
                Checksum = 0,
                ID = ID,
                Sequence = Sequence
            };

            if (Data != null)
            {
                ICMPPacket.Data = Data;
            }

            ICMPPacket.Checksum = (ushort)ChecksumUtils.OnesComplementSum(ICMPPacket.Bytes);
            return ICMPPacket.Bytes;
        }

        public new byte[] ExportAll()
        {
            base.IPProtocolType = IPProtocolType.ICMP;
            base.Payload = Export();
            return base.ExportAll();
        }

        public new void Import(byte[] Bytes)
        {
            var ICMPv4Packet = new ICMPv4Packet(new ByteArraySegment(Bytes));

            TypeCode = ICMPv4Packet.TypeCode;
            ID = ICMPv4Packet.ID;
            Data = ICMPv4Packet.Data;
        }

        public new void ImportAll(byte[] Bytes)
        {
            base.ImportAll(Bytes);
            Import(Payload);
        }

        /*
        public new void Parse(string[] Rows, ref int i)
        {
            // Parse Ethernet
            base.Parse(Rows, ref i);

            // Parse ARP
            if (Rows.Length - i == 0)
            {
                throw new Exception("Expected TypeCode, [ID, [Sequence, [Data]]].");
            }

            TypeCode = (ICMPv4TypeCodes)UInt16.Parse(Rows[i++]);
            ID = UInt16.Parse(Rows[i++].Or("0"));
            Sequence = UInt16.Parse(Rows[i++].Or("0"));

            // String Data
            if (Rows.Length > i && Data == null)
            {
                var String = string.Join("\n", Rows.Skip(i).ToArray());
                Data = System.Text.Encoding.UTF8.GetBytes(String);
            }
        }
        */
    }
}
