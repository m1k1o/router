using System.Collections.Generic;
using System.Net;

namespace Router.Protocols.DHCPOptions
{
    class DHCPDomainNameServerOption : DHCPIPAddressesOption
    {
        public DHCPDomainNameServerOption(string String) : base(DHCPOptionCode.DomainNameServer, String) { }

        public DHCPDomainNameServerOption(byte[] Bytes) : base(DHCPOptionCode.DomainNameServer, Bytes) { }

        public DHCPDomainNameServerOption(List<IPAddress> IPAddresses) : base(DHCPOptionCode.DomainNameServer, IPAddresses) { }

        public DHCPDomainNameServerOption() : base(DHCPOptionCode.DomainNameServer) { }
    }
}
