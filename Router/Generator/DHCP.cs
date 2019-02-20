using Router.Helpers;
using Router.Protocols;
using Router.Protocols.DHCPOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Generator
{
    // TODO: Mega UGLY
    class DHCP : UDP
    {
        public DHCPOperatonCode OperationCode { get; set; }
        public uint TransactionID { get; set; }
        public DHCPMessageType MessageType { get; set; }

        public PhysicalAddress ClientMACAddress { get; set; }
        public IPAddress YourClientIPAddress { get; set; }
        public IPAddress NextServerIPAddress { get; set; }

        public byte[] ClientIdentifier { get; set; }
        public IPAddress RequestedIPAddress { get; set; }
        public List<DHCPOptionCode> ParameterRequestList { get; set; }

        public IPSubnetMask SubnetMask { get; set; }
        public List<IPAddress> Router { get; set; }
        public uint IPAddressLeaseTime { get; set; } = 0;
        public uint RenewalTimeValue { get; set; } = 0;
        public uint RebindingTimeValue { get; set; } = 0;
        public IPAddress ServerIdentifier { get; set; }
        public List<IPAddress> DNS { get; set; }

        public DHCP() { }

        public override PacketDotNet.Packet Export()
        {
            var Options = new DHCPOptionCollection
            {
                new DHCPMessageTypeOption(MessageType)
            };

            /*
             * Client
             */
            if (ClientIdentifier != null)
            {
                Options.Add(new DHCPClientIdentifierOption(ClientIdentifier));
            }

            if (RequestedIPAddress != null)
            {
                Options.Add(new DHCPRequestedIPAddressOption(RequestedIPAddress));
            }

            if (ParameterRequestList != null)
            {
                Options.Add(new DHCPParameterRequestListOption(ParameterRequestList));
            }


            /*
             * Server
             */
            if (SubnetMask != null)
            {
                Options.Add(new DHCPSubnetMaskOption(SubnetMask));
            }

            if (Router != null)
            {
                Options.Add(new DHCPRouterOption(Router));
            }

            if (IPAddressLeaseTime != 0)
            {
                Options.Add(new DHCPIPAddressLeaseTimeOption(IPAddressLeaseTime));
            }

            if (RenewalTimeValue != 0)
            {
                Options.Add(new DHCPRenewalTimeValueOption(RenewalTimeValue));
            }

            if (RebindingTimeValue != 0)
            {
                Options.Add(new DHCPRebindingTimeValueOption(RebindingTimeValue));
            }

            if (ServerIdentifier != null)
            {
                Options.Add(new DHCPServerIdentifierOption(ServerIdentifier));
            }

            if (DNS != null)
            {
                Options.Add(new DHCPDomainNameServerOption(DNS));
            }

            Options.Add(new DHCPEndOption());

            var DHCPPacket = new DHCPPacket(OperationCode, TransactionID, Options);

            if (ClientMACAddress != null)
            {
                DHCPPacket.ClientMACAddress = ClientMACAddress;
            }

            if (YourClientIPAddress != null)
            {
                DHCPPacket.YourClientIPAddress = YourClientIPAddress;
            }

            if (NextServerIPAddress != null)
            {
                DHCPPacket.NextServerIPAddress = NextServerIPAddress;
            }

            // Create UDP Packet
            base.Payload = DHCPPacket.Bytes;
            return base.Export();
        }

        private static List<IPAddress> Optional(string Str)
        {
            if (string.IsNullOrEmpty(Str))
            {
                return null;
            }

            var Result = new List<IPAddress>();

            var Entries = Str.Split(',');
            foreach (var Entry in Entries)
            {
                Result.Add(IPAddress.Parse(Entry));
            }

            return Result;
        }

        private static List<IPAddress> IPs(string Str)
        {
            var Result = new List<IPAddress>();

            var Entries = Str.Split(',');
            foreach (var Entry in Entries)
            {
                Result.Add(IPAddress.Parse(Entry));
            }

            return Result;
        }

        private static List<DHCPOptionCode> Opts(string Str)
        {
            var Result = new List<DHCPOptionCode>();

            var Entries = Str.Split(',');
            foreach (var Entry in Entries)
            {
                Result.Add((DHCPOptionCode)Convert.ToByte(Entry));
            }

            return Result;
        }

        public new void Parse(string[] Rows, ref int i)
        {
            // Parse UDP
            base.Parse(Rows, ref i);

            // Parse DHCP
            if (Rows.Length - i != 16)
            {
                throw new Exception("Expected OperationCode, TransactionID, MessageType, [ClientMACAddress], [YourClientIPAddress], [NextServerIPAddress], [ClientIdentifier(MAC)], [RequestedIPAddress], [ParameterRequestList], [SubnetMask], [Router], [IPAddressLeaseTime], [RenewalTimeValue], [RebindingTimeValue], [ServerIdentifier], [DNS].");
            }

            // Required
            OperationCode = (DHCPOperatonCode)Convert.ToByte(Rows[i++]);// DHCPOperatonCode
            TransactionID = UInt32.Parse(Rows[i++]);// uint
            MessageType = string.IsNullOrEmpty(Rows[i]) ? 0 : (DHCPMessageType)Convert.ToByte(Rows[i++]);// DHCPMessageType

            // Optional
            ClientMACAddress = string.IsNullOrEmpty(Rows[i]) ? null : PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-"));// PhysicalAddress
            YourClientIPAddress = string.IsNullOrEmpty(Rows[i]) ? null : IPAddress.Parse(Rows[i++]);// IPAddress
            NextServerIPAddress = string.IsNullOrEmpty(Rows[i]) ? null : IPAddress.Parse(Rows[i++]);// IPAddress
            ClientIdentifier = string.IsNullOrEmpty(Rows[i]) ? null : PhysicalAddress.Parse(Rows[i++].ToUpper().Replace(":", "-")).GetAddressBytes();// byte[] MAC ADDRESS
            RequestedIPAddress = string.IsNullOrEmpty(Rows[i]) ? null : IPAddress.Parse(Rows[i++]);// IPAddress
            ParameterRequestList = string.IsNullOrEmpty(Rows[i]) ? null : Opts(Rows[i++]);// List<DHCPOptionCode>
            SubnetMask = string.IsNullOrEmpty(Rows[i]) ? null : IPSubnetMask.Parse(Rows[i++]);// IPSubnetMask
            Router = string.IsNullOrEmpty(Rows[i]) ? null : IPs(Rows[i++]);// List<IPAddress>
            IPAddressLeaseTime = string.IsNullOrEmpty(Rows[i]) ? (uint)0 : UInt16.Parse(Rows[i++]);// uint
            RenewalTimeValue = string.IsNullOrEmpty(Rows[i]) ? (uint)0 : UInt16.Parse(Rows[i++]);// uint
            RebindingTimeValue = string.IsNullOrEmpty(Rows[i]) ? (uint)0 : UInt16.Parse(Rows[i++]);// uint
            ServerIdentifier = string.IsNullOrEmpty(Rows[i]) ? null : IPAddress.Parse(Rows[i++]);// IPAddress
            DNS = string.IsNullOrEmpty(Rows[i]) ? null : IPs(Rows[i++]);// List<IPAddress>
        }
    }
}
