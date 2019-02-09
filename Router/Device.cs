using System;
using System.Net;
using System.Net.NetworkInformation;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;

namespace Router
{
    abstract class Device
    {

        private ICaptureDevice ICaptureDevice;
        public string Name { get => ICaptureDevice.Name; }
        public string FriendlyName { get => ICaptureDevice.ToString().Split('\n')[1].Substring(14); }
        public string Description { get => ICaptureDevice.Description; }
        public PhysicalAddress PhysicalAddress { get; private set; }
        public IPAddress DeviceIP { get; private set; } = null;
        public bool Running { get; set; } = false;

        protected Device(ICaptureDevice ICaptureDevice)
        {
            this.ICaptureDevice = ICaptureDevice;

            // Get MAC from only opened device
            this.ICaptureDevice.Open();
            PhysicalAddress = this.ICaptureDevice.MacAddress;

            var ZeroIp = IPAddress.Parse("0.0.0.0");
            foreach (PcapAddress addr in (this.ICaptureDevice as WinPcapDevice).Addresses)
            {
                if (
                    addr.Addr != null &&
                    addr.Addr.ipAddress != null &&
                    addr.Addr.ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                    !Equals(addr.Addr.ipAddress, ZeroIp)
                )
                {
                    DeviceIP = addr.Addr.ipAddress;
                }
            }

            this.ICaptureDevice.Close();
        }

        protected Action<RawCapture> OnPacketArrival;
        protected Action BeforeStarted;
        protected Action AfterStarted;
        protected Action AfterStopped;

        private void SetFilter()
        {
            var DeviceMac = BitConverter.ToString(PhysicalAddress.GetAddressBytes()).Replace("-", ":");
            var RequestDestinationMac = BitConverter.ToString(Protocols.ARP.RequestDestinationMac.GetAddressBytes()).Replace("-", ":");
            var MulticastMac = BitConverter.ToString(Protocols.RIP.MulticastMac.GetAddressBytes()).Replace("-", ":");

            String Filter = "(" +
                "ether dst " + DeviceMac + " or " +
                "ether dst " + RequestDestinationMac + " or " +
                "ether dst " + MulticastMac +
            ") and (ip or arp) ";


            if (DeviceIP != null)
            {
                Filter += " and (not dst host " + DeviceIP + ")";
            }

            ICaptureDevice.Filter = Filter;
        }

        public void Start()
        {
            if (Running)
            {
                throw new Exception("Interface is already running.");
            }

            BeforeStarted();

            ICaptureDevice.Open(DeviceMode.Promiscuous, 1);
            SetFilter();

            ICaptureDevice.OnPacketArrival += (object sender, CaptureEventArgs e) =>
            {
                if (!Running)
                {
                    return;
                }

                OnPacketArrival(e.Packet);
            };

            ICaptureDevice.OnCaptureStopped += (object sender, CaptureStoppedEventStatus e) =>
            {
                if(Running)
                {
                    Running = false;
                    AfterStopped();
                }
            };

            ICaptureDevice.StartCapture();
            
            Running = true;
            AfterStarted();
        }

        public void Stop()
        {
            if (!Running)
            {
                throw new Exception("Interface is not running.");
            }

            try
            {
                ICaptureDevice.StopCapture();
            }
            catch { };

            try
            {

                ICaptureDevice.Close();
            }
            catch { };

            Running = false;
            AfterStopped();
        }

        public void SendPacket(byte[] Data)
        {
            ICaptureDevice.SendPacket(Data);
        }
    }
}