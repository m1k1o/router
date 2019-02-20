using PacketDotNet;
using System;
using System.Linq;

namespace Router.Generator
{
    class UDP : IP, Generator
    {
        public ushort SourcePort { get; set; }
        public ushort DestinationPort { get; set; }

        public virtual byte[] Payload { get; set; }

        public UDP() { }

        public virtual Packet Export()
        {
            // Export UDP
            var UdpPacket = new UdpPacket(SourcePort, DestinationPort);

            if(Payload != null)
            {
                UdpPacket.PayloadData = Payload;
            }

            // Export IP
            return base.Export(IPProtocolType.UDP, UdpPacket);
        }

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
    }
}
