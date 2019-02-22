using System.Net;

namespace Router.Protocols.DHCPOptions
{
    class DHCPRequestedIPAddressOption : DHCPIPAddressOption
    {
        public DHCPRequestedIPAddressOption(string String) : base(DHCPOptionCode.RequestedIPAddress, String) { }

        public DHCPRequestedIPAddressOption(byte[] Bytes) : base(DHCPOptionCode.RequestedIPAddress, Bytes) { }

        public DHCPRequestedIPAddressOption(IPAddress IPAddress) : base(DHCPOptionCode.RequestedIPAddress, IPAddress) { }
    }
}
