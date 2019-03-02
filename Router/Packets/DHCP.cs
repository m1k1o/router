using Router.Protocols;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    // TODO: Payload
    sealed class DHCP : GeneratorPacket
    {
        public DHCPOperatonCode OperationCode { get; set; } = 0;
        public uint TransactionID { get; set; } = 0;
        public IPAddress YourClientIPAddress { get; set; } = IPAddress.Parse("0.0.0.0");
        public IPAddress NextServerIPAddress { get; set; } = IPAddress.Parse("0.0.0.0");
        public PhysicalAddress ClientMACAddress { get; set; } = PhysicalAddress.Parse("00-00-00-00-00-00");

        public string MessageType => Options == null ? "BOOTP" : "DHCP " + Options.MessageType.ToString();
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
