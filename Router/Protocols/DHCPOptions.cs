namespace Router.Protocols
{
    internal class DHCPOptions
    {
        private byte[] RawData;

        public DHCPOptions(byte[] RawData)
        {
            this.RawData = RawData;
        }

        public byte[] Bytes { get; private set; } = new byte[0] { };

        public int Length { get; private set; } = 0;
    }
}