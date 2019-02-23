using Router.Helpers;
using Router.Protocols;
using Router.Protocols.DHCPOptions;
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    class DHCP : UDP, Generator
    {
        public DHCPOperatonCode OperationCode { get; set; }
        public uint TransactionID { get; set; }
        public IPAddress YourClientIPAddress { get; set; }
        public IPAddress NextServerIPAddress { get; set; }
        public PhysicalAddress ClientMACAddress { get; set; }

        public DHCPOptionCollection Options { get; set; }

        public DHCP() { }

        public override PacketDotNet.Packet Export()
        {
            // Create DHCP Packet
            var DHCPPacket = new DHCPPacket(OperationCode, TransactionID, Options);

            if (YourClientIPAddress != null)
                DHCPPacket.YourClientIPAddress = YourClientIPAddress;
            if (NextServerIPAddress != null)
                DHCPPacket.NextServerIPAddress = NextServerIPAddress;
            if (ClientMACAddress != null)
                DHCPPacket.ClientMACAddress = ClientMACAddress;

            // Create UDP Packet
            base.Payload = DHCPPacket.Bytes;
            return base.Export();
        }

        public new void Parse(string[] Rows, ref int i)
        {
            // Parse UDP
            base.Parse(Rows, ref i);

            // Parse DHCP
            if (Rows.Length - i < 5)
            {
                throw new Exception("Expected OperationCode, TransactionID, YourClientIPAddress, NextServerIPAddress, ClientMACAddress, [DHCP Options].");
            }

            // BOOTP
            OperationCode = (DHCPOperatonCode)Convert.ToByte(Rows[i++]);
            TransactionID = UInt32.Parse(Rows[i++].Or("0"));
            YourClientIPAddress = IPAddress.Parse(Rows[i++].Or("0.0.0.0"));
            NextServerIPAddress = IPAddress.Parse(Rows[i++].Or("0.0.0.0"));
            ClientMACAddress = Utilities.ParseMAC(Rows[i++].Or("00-00-00-00-00-00"));

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
    }
}
