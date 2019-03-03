namespace Router.Packets
{
    sealed class Payload: GeneratorPacket
    {
        public string String
        {
            /*get
            {
                try
                {
                    return System.Text.Encoding.UTF8.GetString(Data);
                }
                catch
                {
                    return null;
                }
            }*/
            set
            {
                try
                {
                    Data = System.Text.Encoding.UTF8.GetBytes(value);
                }
                catch
                { }
            }
        }

        public byte[] Data { get; set; }

        public Payload() { }

        public override byte[] Export() => Data;

        public override void Import(byte[] Bytes)
        {
            if (Bytes == null) return;
            Data = Bytes;
        }
    }
}
