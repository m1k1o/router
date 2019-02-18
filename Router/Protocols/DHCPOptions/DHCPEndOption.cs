namespace Router.Protocols.DHCPOptions
{
    class DHCPEndOption : DHCPOption
    {
        public DHCPEndOption() : base(DHCPOptionCode.End) { }

        public override byte[] Bytes => new byte[] { 255 };
    }
}
