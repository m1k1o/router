using PacketDotNet;
using PacketDotNet.Utils;
using System.Net;

namespace Router.Packets
{
    abstract class IP : Ethernet
    {
        public IPAddress SourceAddress { get; set; }
        public IPAddress DestinationAddress { get; set; }
        public int TimeToLive { get; set; }

        public virtual IPProtocolType IPProtocolType { get; set; }
        public virtual new byte[] Payload { get; set; }

        protected IP() { }

        public new byte[] Export()
        {
            var IPv4Packet = new IPv4Packet(SourceAddress, DestinationAddress)
            {
                TimeToLive = TimeToLive,
                Protocol = IPProtocolType,
                PayloadData = Payload
            };

            IPv4Packet.Checksum = IPv4Packet.CalculateIPChecksum();
            return IPv4Packet.Bytes;
        }

        public new byte[] ExportAll()
        {
            base.EthernetPacketType = EthernetPacketType.IpV4;
            base.Payload = Export();
            return base.ExportAll();
        }

        public new void Import(byte[] Bytes)
        {
            var IPv4Packet = new IPv4Packet(new ByteArraySegment(Bytes));

            SourceAddress = IPv4Packet.SourceAddress;
            DestinationAddress = IPv4Packet.DestinationAddress;
            TimeToLive = IPv4Packet.TimeToLive;
            IPProtocolType = IPv4Packet.Protocol;
            Payload = IPv4Packet.PayloadData;
        }

        public new void ImportAll(byte[] Bytes)
        {
            base.ImportAll(Bytes);
            Import(Payload);
        }

        /*
        protected new void Parse(string[] Rows, ref int i)
        {
            // Parse Ethernet
            base.Parse(Rows, ref i);

            // Parse IP
            if (Rows.Length - i < 4)
            {
                throw new Exception("Expected SourceAddress, DestinationAddress, TimeToLive.");
            }

            SourceAddress = IPAddress.Parse(Rows[i++]);
            DestinationAddress = IPAddress.Parse(Rows[i++]);
            TimeToLive = Int32.Parse(Rows[i++]);
        }
        */
    }
}
