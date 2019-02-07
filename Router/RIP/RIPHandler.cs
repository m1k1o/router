using Router.Protocols;
using System;
using System.Collections.Generic;

namespace Router.RIP
{
    class RIPHandler
    {
        private List<Interface> Interfaces = new List<Interface>();

        static public void OnReceived(RIPPacket RIPPacket, Interface Interface)
        {
            if (RIPPacket.CommandType == RIPCommandType.Request)
            {
                throw new NotImplementedException();
            }

            if (RIPPacket.CommandType == RIPCommandType.Response)
            {
                throw new NotImplementedException();
            }
        }
    }
}
