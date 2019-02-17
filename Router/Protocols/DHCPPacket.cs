using Router.Helpers;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Router.Protocols
{
    sealed class DHCPPacket : Packet
    {
        private const uint DHCP_MAGIC_COOKIE = 0x63825363;

        // Message op code.
        public DHCPOperatonCode OperationCode
        {
            get => (DHCPOperatonCode)Slice(0, typeof(byte));
            set => Inject(0, (byte)value);
        }

        // Hardware address type.
        public PacketDotNet.LinkLayers HardwareType
        {
            get => (PacketDotNet.LinkLayers)Slice(1, typeof(byte));
            set => Inject(1, (byte)value);
        }

        // Hardware address length.
        public byte HardwareAddressLength
        {
            get => (byte)Slice(2, typeof(byte));
            set => Inject(2, value);
        }

        // Client sets to zero, optionally used by relay agents when booting via a relay agent.
        public byte Hops
        {
            get => (byte)Slice(3, typeof(byte));
            set => Inject(3, value);
        }

        // Transaction ID, a random number chosen by the client, used by the client and server to associate messages and responses between a client and a server.
        public uint TransactionID
        {
            get => (uint)Slice(4, typeof(uint));
            set => Inject(4, value);
        }

        // Filled in by client, seconds elapsed since client began address acquisition or renewal process.
        public ushort Seconds
        {
            get => (ushort)Slice(8, typeof(ushort));
            set => Inject(8, value);
        }

        // Flags.
        public DHCPFlags Flags
        {
            get => (DHCPFlags)Slice(10, typeof(ushort));
            set => Inject(10, (ushort)value);
        }

        // Client IP address; only filled in if client is in BOUND, RENEW or REBINDING state and can respond to ARP requests.
        public IPAddress ClientIPAddress
        {
            get => (IPAddress)Slice(12, typeof(IPAddress));
            set => Inject(12, value);
        }

        // 'your' (client) IP address.
        public IPAddress YourClientIPAddress
        {
            get => (IPAddress)Slice(16, typeof(IPAddress));
            set => Inject(16, value);
        }

        // IP address of next server to use in bootstrap; returned in DHCPOFFER, DHCPACK by server.
        public IPAddress NextServerIPAddress
        {
            get => (IPAddress)Slice(20, typeof(IPAddress));
            set => Inject(20, value);
        }

        // Relay agent IP address, used in booting via a relay agent.
        public IPAddress RelayAgentIPAddress
        {
            get => (IPAddress)Slice(24, typeof(IPAddress));
            set => Inject(24, value);
        }

        // Client hardware address.
        public byte[] ClientHardwareAddress
        {
            get => Slice(28, 16);
            set => Inject(28, value, 16);
        }

        // Client MAC address.
        public PhysicalAddress ClientMACAddress
        {
            get => (PhysicalAddress)Slice(28, typeof(PhysicalAddress));
            set => Inject(28, value);
        }

        // Optional server host name.
        public string ServerName
        {
            get
            {
                // TODO: endianness
                var _ServerName = Slice(44, 64);
                return Encoding.ASCII.GetString(_ServerName).TrimEnd('\0');
            }
            set
            {
                // TODO: endianness
                var _ServerName = Encoding.ASCII.GetBytes(value);
                Inject(44, _ServerName, 64);
            }
        }

        // Boot file name; "generic" name or null in DHCPDISCOVER, fully qualified directory-path name in DHCPOFFER.
        public string BootFilename
        {
            get
            {
                // TODO: endianness
                var _BootFilename = Slice(108, 128);
                return Encoding.ASCII.GetString(_BootFilename).TrimEnd('\0');
            }
            set
            {
                // TODO: endianness
                var _BootFilename = Encoding.ASCII.GetBytes(value);
                Inject(108, _BootFilename, 128);
            }
        }

        // Whether the magic dhcp-cookie is set. If set this datagram is a dhcp-datagram. Otherwise it's a bootp-datagram.
        public bool IsDHCP
        {
            get
            {
                if (Length >= 240)
                {
                    return (uint)Slice(236, typeof(uint)) == DHCP_MAGIC_COOKIE;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    if (Length < 240)
                    {
                        Expand(240);
                    }

                    Inject(236, DHCP_MAGIC_COOKIE);
                }
                else
                {
                    if (Length >= 240)
                    {
                        Inject(236, (uint)0);
                    }
                }
            }
        }

        // Optional parameters field.
        public DHCPOptionCollection Options
        {
            get => new DHCPOptionCollection(Slice(240, Length - 240));
            set => Inject(240, value.Bytes, value.Length);
        }

        public DHCPPacket(DHCPOperatonCode OperationCode, uint TransactionID, DHCPOptionCollection Options) : base(240 + Options.Length)
        {
            this.OperationCode = OperationCode;
            HardwareType = PacketDotNet.LinkLayers.Ethernet;
            HardwareAddressLength = 6;
            this.TransactionID = TransactionID;
            IsDHCP = true;
            this.Options = Options;
        }

        public DHCPPacket(byte[] Data) : base(Data) { }
    }
}
