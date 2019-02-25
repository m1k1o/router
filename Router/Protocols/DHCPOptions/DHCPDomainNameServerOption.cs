using System.Collections.Generic;
using System.Net;

namespace Router.Protocols.DHCPOptions
{
    class DHCPDomainNameServerOption : DHCPIPAddressesOption
    {
        public DHCPDomainNameServerOption(byte[] Bytes) : base(DHCPOptionCode.DomainNameServer, Bytes) { }

        public DHCPDomainNameServerOption(List<IPAddress> IPAddresses) : base(DHCPOptionCode.DomainNameServer, IPAddresses) { }
    }
}
