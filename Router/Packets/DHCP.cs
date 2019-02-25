using Router.Protocols;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Packets
{
    class DHCP : GeneratorPacket
    {
        public DHCPOperatonCode OperationCode { get; set; }
        public uint TransactionID { get; set; }
        public IPAddress YourClientIPAddress { get; set; }
        public IPAddress NextServerIPAddress { get; set; }
        public PhysicalAddress ClientMACAddress { get; set; }

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
            var DHCPPacket = new DHCPPacket(Bytes);

            OperationCode = DHCPPacket.OperationCode;
            TransactionID = DHCPPacket.TransactionID;
            YourClientIPAddress = DHCPPacket.YourClientIPAddress;
            NextServerIPAddress = DHCPPacket.NextServerIPAddress;
            ClientMACAddress = DHCPPacket.ClientMACAddress;
            Options = DHCPPacket.Options;
        }
        
        /*
        public new void Parse(string[] Rows, ref int i)
        {
            // DHCP Options
            Options = new DHCPOptionCollection();
            while (i < Rows.Length - 1)
            {
                var Instance = DHCPOption.Factory(Convert.ToByte(Rows[i++]));
                Instance.Parse(Rows[i++]);
                Options.Add(Instance);
            }

            Options.Add(new DHCPEndOption());
        }
        */
    }
}
