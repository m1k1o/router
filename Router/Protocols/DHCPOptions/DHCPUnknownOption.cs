namespace Router.Protocols.DHCPOptions
{
    class DHCPUnknownOption : DHCPOption
    {
        private byte[] RawData;

        public DHCPUnknownOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            RawData = Bytes;
        }

        public override byte[] Bytes => RawData;

        public override void Parse(string String) { }
    }
}
