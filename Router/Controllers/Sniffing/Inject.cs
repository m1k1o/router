using Router.Packets;
using System;

namespace Router.Controllers.Sniffing
{
    class Inject : Controller, Executable
    {
        public Router.Interface Interface { get; set; }

        public GeneratorPacket Packet { get; set; }

        public void Execute()
        {
            if(Interface == null || Packet == null)
            {
                throw new Exception("Expected Interface and Packet.");
            }

            Interface.SendPacket(Packet.Export());
        }

        public object Export() => this;
    }
}
