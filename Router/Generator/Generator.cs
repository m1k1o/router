namespace Router.Generator
{
    interface Generator
    {
        PacketDotNet.Packet Export();

        void Parse(string Data);
    }
}
