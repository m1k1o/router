﻿using System.Net;

namespace Router.Protocols.DHCPOptions
{
    class DHCPSubnetMaskOption : DHCPIPAddressOption
    {
        public DHCPSubnetMaskOption(string String) : base(DHCPOptionCode.SubnetMask, String) { }

        public DHCPSubnetMaskOption(byte[] Bytes) : base(DHCPOptionCode.SubnetMask, Bytes) { }

        public DHCPSubnetMaskOption(IPAddress IPAddress) : base(DHCPOptionCode.SubnetMask, IPAddress) { }
    }
}
