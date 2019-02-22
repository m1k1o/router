using System.Net;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPIPAddressOption : DHCPOption
    {
        public IPAddress IPAddress { get; private set; }

        public DHCPIPAddressOption(DHCPOptionCode DHCPOptionCode, string String) : base(DHCPOptionCode)
        {
            IPAddress = IPAddress.Parse(String);
        }

        public DHCPIPAddressOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            IPAddress = new IPAddress(Bytes);
        }

        public DHCPIPAddressOption(DHCPOptionCode DHCPOptionCode, IPAddress IPAddress) : base(DHCPOptionCode)
        {
            this.IPAddress = IPAddress;
        }

        public override byte[] Bytes
        {
            get => IPAddress.GetAddressBytes();
        }
    }
}
