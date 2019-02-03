using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet;
using SharpPcap;

namespace Router
{
    internal class Interface
    {
        public ICaptureDevice Device { get; private set; }

        public IPAddress IPAddress { get; set; }

        public PhysicalAddress PhysicalAddress { get; set; }

        public bool Selected { get; set; } = false;

        public string Name { get => Device.Name; }

        public Interface(ICaptureDevice ICaptureDevice)
        {
            Device = ICaptureDevice;
        }

        internal void SendPacket(byte[] Data)
        {
            Device.SendPacket(Data);
        }
    }
}