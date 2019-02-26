namespace Router.Protocols.DHCPOptions
{
    class DHCPUnknownOption : DHCPOption
    {
        public byte[] Value { get; set; }

        public DHCPUnknownOption(DHCPOptionCode DHCPOptionCode) : base(DHCPOptionCode)
        {
            Value = new byte[0];
        }

        public DHCPUnknownOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            Value = Bytes;
        }

        public override byte[] Bytes => Value;
    }
}
