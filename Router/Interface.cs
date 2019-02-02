using System;
using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet;

namespace Router
{
    internal class Interface
    {
        internal void Open()
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            throw new NotImplementedException();
        }

        internal void SendPacket(Packet payloadPacket)
        {
            throw new NotImplementedException();
        }

        internal PhysicalAddress GetMac()
        {
            throw new NotImplementedException();
        }

        internal IPAddress GetIp()
        {
            throw new NotImplementedException();
        }
    }
}