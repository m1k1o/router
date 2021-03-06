﻿using System.Net;

namespace Router.Protocols.DHCPOptions
{
    class DHCPServerIdentifierOption : DHCPIPAddressOption
    {
        public DHCPServerIdentifierOption() : base(DHCPOptionCode.ServerIdentifier) { }

        public DHCPServerIdentifierOption(byte[] Bytes) : base(DHCPOptionCode.ServerIdentifier, Bytes) { }

        public DHCPServerIdentifierOption(IPAddress IPAddress) : base(DHCPOptionCode.ServerIdentifier, IPAddress) { }
    }
}
