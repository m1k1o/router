using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    class UDP : IP, Generator
    {
        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }

        public virtual new byte[] Payload { get; set; }

        public UDP() { }

        public new byte[] Export()
        {
            var UdpPacket = new UdpPacket(SourcePort, DestinationPort)
            {
                PayloadData = Payload
            };

            return UdpPacket.Bytes;
        }

        public new byte[] ExportAll()
        {
            base.IPProtocolType = IPProtocolType.UDP;
            base.Payload = Export();
            return base.ExportAll();
        }

        public new void Import(byte[] Bytes)
        {
            var UdpPacket = new UdpPacket(new ByteArraySegment(Bytes));

            SourcePort = UdpPacket.SourcePort;
            DestinationPort = UdpPacket.DestinationPort;
            Payload = UdpPacket.PayloadData;
        }

        public new void ImportAll(byte[] Bytes)
        {
            base.ImportAll(Bytes);
            Import(Payload);
        }

        /*
        public new void Parse(string[] Rows, ref int i)
        {
            // Parse IP
            base.Parse(Rows, ref i);

            if (Rows.Length - i < 3)
            {
                throw new Exception("Expected SourcePort, DestinationPort, [Payload].");
            }

            SourcePort = UInt16.Parse(Rows[i++]);
            DestinationPort = UInt16.Parse(Rows[i++]);

            // String Payload
            if (Rows.Length > i && Payload == null)
            {
                var String = string.Join("\n", Rows.Skip(i).ToArray());
                Payload = System.Text.Encoding.UTF8.GetBytes(String);
            }
        }
        */
    }
}
