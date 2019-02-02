using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet;
using SharpPcap;

namespace Router
{
    internal class Interface
    {
        private ICaptureDevice Device;
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

        public Interface(ICaptureDevice Device)
        {
            this.Device = Device;
        }

        internal void Open()
        {
            Device.Open();
        }

        internal void Close()
        {
            Device.Close();
        }

        internal void SendPacket(Packet payloadPacket)
        {
            Device.SendPacket(payloadPacket);
        }
    }
}