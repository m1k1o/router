namespace Router.Protocols.DHCPOptions
{
    class DHCPPadOption : DHCPOption
    {
        public int Size { get; set; } = 0;

        public DHCPPadOption() : base(DHCPOptionCode.Pad)
        {
            this.Size = Size;
        }

        public DHCPPadOption(byte[] Bytes) : base(DHCPOptionCode.Pad)
        {
            Size = Bytes.Length;
        }

        public DHCPPadOption(int Size) : base(DHCPOptionCode.Pad)
        {
            this.Size = Size;
        }

        public override byte[] Bytes => new byte[Size];
    }
}
