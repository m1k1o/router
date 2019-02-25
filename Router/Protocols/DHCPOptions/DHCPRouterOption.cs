using System.Collections.Generic;
using System.Net;

namespace Router.Protocols.DHCPOptions
{
    class DHCPRouterOption : DHCPIPAddressesOption
    {
        public DHCPRouterOption(byte[] Bytes) : base(DHCPOptionCode.Router, Bytes) { }

        public DHCPRouterOption(List<IPAddress> IPAddresses) : base(DHCPOptionCode.Router, IPAddresses) { }
    }
}
