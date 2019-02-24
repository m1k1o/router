using PacketDotNet;
using System;
using System.Linq;

namespace Router.Generator
{
    class TCP : IP, Generator
    {
        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }

        public byte Flags { get; set; }

        public virtual new byte[] Payload { get; set; }

        public TCP() { }

        public new byte[] Export()
        {
            var TcpPacket = new TcpPacket(SourcePort, DestinationPort)
            {
                AllFlags = Flags,
                PayloadData = Payload
            };

            return TcpPacket.Bytes;
        }

        public new byte[] ExportAll()
        {
            base.IPProtocolType = IPProtocolType.TCP;
            base.Payload = Export();
            return base.ExportAll();
        }

        /*
        public new void Parse(string[] Rows, ref int i)
        {
            // Parse IP
            base.Parse(Rows, ref i);

            if (Rows.Length - i < 3)
            {
                throw new Exception("Expected SourcePort, DestinationPort, Flags, [Payload].");
            }

            SourcePort = UInt16.Parse(Rows[i++]);
            DestinationPort = UInt16.Parse(Rows[i++]);
            Flags = Convert.ToByte(Rows[i++]);

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
