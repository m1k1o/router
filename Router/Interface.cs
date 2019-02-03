using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet;
using SharpPcap;

namespace Router
{
    internal class Interface
    {
        private ICaptureDevice ICaptureDevice;
        public ICaptureDevice Device { get => ICaptureDevice; }

        private IPAddress ipAddress;
        private PhysicalAddress physicalAddress;

        public IPAddress IPAddress {
            get => ipAddress;

            set
            {
                // Update RT
                ipAddress = value;
            }
        }

        public PhysicalAddress PhysicalAddress
        {
            get => physicalAddress;

            set
            {
                physicalAddress = value;
            }
        }

        public string Name { get => Device.Name; }

        public Interface(ICaptureDevice ICaptureDevice)
        {
            this.ICaptureDevice = ICaptureDevice;
        }

        internal void Open()
        {
            Device.Open();
        }

        internal void Close()
        {
            Device.Close();
        }

        internal void SendPacket(byte[] Data)
        {
            Device.SendPacket(Data);
        }
    }
}