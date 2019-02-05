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
            IPNetwork = IPNetwork.Parse(IPAddress, SubnetMask);
            this.IPAddress = IPAddress;

            RoutingTable.Instance.Push(this, IPNetwork);
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

        public bool Equals(Interface Interface)
        {
            if (Interface is null)
            {
                return false;
            }

            if (ReferenceEquals(this, Interface))
            {
                return true;
            }

            return Equals(Interface.Name, Name);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == GetType() && Equals(obj as Interface);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Interface obj1, Interface obj2)
        {
            if (obj1 is null || obj2 is null)
            {
                return false;
            }

            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(Interface obj1, Interface obj2)
        {
            return !(obj1 == obj2);
        }
    }
}