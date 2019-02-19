using PacketDotNet;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    class TCP : Generator
    {
        public PhysicalAddress SourceHwAddress { get; set; }
        public PhysicalAddress DestinationHwAddress { get; set; }

        public IPAddress DestinationAddress { get; set; }
        public IPAddress SourceAddress { get; set; }

        public ushort DestinationPort { get; set; }
        public ushort SourcePort { get; set; }

        public virtual byte[] Payload { get; set; }

        public TCP() { }

        public virtual Packet Export()
        {
            var TcpPacket = new TcpPacket(SourcePort, DestinationPort);
            
            if (Payload != null)
            {
                TcpPacket.PayloadData = Payload;
            }

            var IPv4Packet = new IPv4Packet(SourceAddress, DestinationAddress)
            {
                PayloadPacket = TcpPacket
            };
            IPv4Packet.Checksum = IPv4Packet.CalculateIPChecksum();

            var EthernetPacket = new EthernetPacket(SourceHwAddress, DestinationHwAddress, EthernetPacketType.IpV4)
            {
                PayloadPacket = IPv4Packet
            };

            return EthernetPacket;
        }

        public virtual void Parse(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length < 6)
            {
                throw new Exception("Expected SourceHwAddress, DestinationHwAddress, SourceAddress, DestinationAddress, SourcePort, DestinationPort, [Payload].");
            }

            var i = 0;
            SourceHwAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            DestinationHwAddress = PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));
            SourceAddress = IPAddress.Parse(Rows[i++]);
            DestinationAddress = IPAddress.Parse(Rows[i++]);
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
