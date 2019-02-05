using System.Net;
using System.Net.NetworkInformation;
using PacketDotNet;
using Router.Helpers;
using SharpPcap;

namespace Router
{
    internal class Interface
    {
        public ICaptureDevice Device { get; private set; }

        public IPAddress IPAddress { get; private set; }

        public IPNetwork IPNetwork { get; private set; }

        public PhysicalAddress PhysicalAddress { get; private set; }

        public bool Running { get; set; } = false;
        public int ID { get; private set; }

        public string Name { get => Device.Name; }

        public string FriendlyName { get => Device.ToString().Split('\n')[1].Substring(14); }
        
        public string Description { get => Device.Description; }

        public Interface(ICaptureDevice ICaptureDevice, int ID)
        {
            Device = ICaptureDevice;
            this.ID = ID;

            // Get MAC from only opened device
            Device.Open();
            PhysicalAddress = Device.MacAddress;
            Device.Close();
        }

        public void SetIP(IPAddress IPAddress, IPAddress SubnetMask)
        {
            this.IPNetwork = IPNetwork.Parse(IPAddress, SubnetMask);
            this.IPAddress = IPAddress;
        }

        public void Start()
        {
            Device.Open(DeviceMode.Promiscuous, 1);
            Device.OnPacketArrival += new PacketArrivalEventHandler(Interfaces.OnPacketArrival);
            Device.OnCaptureStopped += new CaptureStoppedEventHandler(Interfaces.OnCaptureStopped);
            Device.StartCapture();

            Running = true;
        }

        public void Stop()
        {
            try
            {
                Device.StopCapture();
            }
            catch { };

            try
            {

                Device.Close();
            }
            catch { };

            Running = false;
        }

        internal void SendPacket(byte[] Data)
        {
            Device.SendPacket(Data);
        }

        public override string ToString()
        {
            return ID.ToString();
        }
    }
}