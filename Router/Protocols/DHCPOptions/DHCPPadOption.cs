namespace Router.Protocols.DHCPOptions
{
    class DHCPPadOption : DHCPOption
    {
        private int Size = 0;

        public DHCPPadOption(byte[] Bytes) : base(DHCPOptionCode.Pad)
        {
            Size = Bytes.Length;
        }

        public DHCPPadOption(int Size) : base(DHCPOptionCode.Pad)
        {
            this.Size = Size;
        }

        public DHCPPadOption() : base(DHCPOptionCode.Pad) { }

        public override byte[] Bytes => new byte[Size];

        public override void Parse(string String) { }
    }
}
