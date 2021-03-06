﻿namespace Router.Protocols.DHCPOptions
{
    class DHCPUnknownOption : DHCPOption
    {
        public byte[] RawData { get; set; }

        public DHCPUnknownOption() : base(0) { }

        public DHCPUnknownOption(DHCPOptionCode DHCPOptionCode) : base(DHCPOptionCode)
        {
            RawData = new byte[0];
        }

        public DHCPUnknownOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            RawData = Bytes;
        }

        public override byte[] Bytes => RawData;
    }
}
