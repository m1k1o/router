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

        public IPAddress Mask { get; set; }

        public PhysicalAddress PhysicalAddress { get; private set; }

        public bool Selected { get; set; } = false;

        public string Name { get => Device.Name; }

        public string FriendlyName { get => Device.ToString().Split('\n')[1].Substring(14); }
        
        public string Description { get => Device.Description; }

        public Interface(ICaptureDevice ICaptureDevice)
        {
            Device = ICaptureDevice;

            // Get MAC from only opened device
            Device.Open();
            PhysicalAddress = Device.MacAddress;
            Device.Close();
        }

        internal void SendPacket(byte[] Data)
        {
            Device.SendPacket(Data);
        }
    }
}