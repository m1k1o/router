using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Protocols;
using Router.Protocols.DHCPOptions;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    class DHCP : GeneratorPacket
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DHCPOperatonCode OperationCode { get; set; }
        public uint TransactionID { get; set; }
        public IPAddress YourClientIPAddress { get; set; }
        public IPAddress NextServerIPAddress { get; set; }
        public PhysicalAddress ClientMACAddress { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DHCPMessageType MessageType => Options == null ? 0 : Options.MessageType;
        public DHCPOptionCollection Options { get; set; } = new DHCPOptionCollection();

        public DHCP() { }

        public override byte[] Export()
        {
            var DHCPPacket = new DHCPPacket(OperationCode, TransactionID, Options);

            if (YourClientIPAddress != null)
                DHCPPacket.YourClientIPAddress = YourClientIPAddress;
            if (NextServerIPAddress != null)
                DHCPPacket.NextServerIPAddress = NextServerIPAddress;
            if (ClientMACAddress != null)
                DHCPPacket.ClientMACAddress = ClientMACAddress;

            return DHCPPacket.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            if (Bytes == null) return;

            var DHCPPacket = new DHCPPacket(Bytes);

            OperationCode = DHCPPacket.OperationCode;
            TransactionID = DHCPPacket.TransactionID;
            YourClientIPAddress = DHCPPacket.YourClientIPAddress;
            NextServerIPAddress = DHCPPacket.NextServerIPAddress;
            ClientMACAddress = DHCPPacket.ClientMACAddress;
            Options = DHCPPacket.Options;
        }
    }
}
