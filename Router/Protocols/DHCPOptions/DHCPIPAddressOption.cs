﻿using System.Net;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPIPAddressOption : DHCPOption
    {
        public IPAddress IPAddress { get; set; }

        public DHCPIPAddressOption(DHCPOptionCode DHCPOptionCode) : base(DHCPOptionCode)
        {
            IPAddress = new IPAddress(0);
        }

        public DHCPIPAddressOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            IPAddress = new IPAddress(Bytes);
        }

        public DHCPIPAddressOption(DHCPOptionCode DHCPOptionCode, IPAddress IPAddress) : base(DHCPOptionCode)
        {
            this.IPAddress = IPAddress;
        }

        public override byte[] Bytes => IPAddress.GetAddressBytes();
    }
}
