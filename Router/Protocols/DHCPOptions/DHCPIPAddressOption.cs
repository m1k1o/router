using System.Net;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPIPAddressOption : DHCPOption
    {
        private IPAddress IPAddress;

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
