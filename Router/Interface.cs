using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Router.Helpers;
using SharpPcap;

namespace Router
{
    delegate void InterfaceEvent(Interface Interface);
    delegate void PacketArrival(Handler Handler);

    class Interface : Device
    {
        public int ID { get; private set; }

        public event InterfaceEvent OnStarted;
        public event InterfaceEvent OnStopped;
        public event InterfaceEvent OnChanged;
        public event PacketArrival OnPacketArrival;

        public IPAddress IPAddress { get; private set; }
        public IPNetwork IPNetwork { get; private set; }

        public Interface(ICaptureDevice ICaptureDevice, int ID) : base(ICaptureDevice)
        {
            this.ID = ID;

            PacketArrival = (Packet) => OnPacketArrival(new Handler(Packet, this));

            BeforeStarted = () =>
            {
                if (IPNetwork == null)
                {
                    throw new Exception("You must first set IPAddress and IPSubnetMask.");
                }

                if (RoutingTable.Instance.Find(IPNetwork, ADistance.DirectlyConnected) != null)
                {
                    throw new Exception("There is already C" + IPNetwork + " in routing table.");
                }
            };

            AfterStarted = () => OnStarted(this);

            AfterStopped = () => OnStopped(this);

            ServicesInitialize();
        }

        public void SetIP(IPAddress IPAddress, IPSubnetMask IPSubnetMask)
        {
            IPNetwork = IPNetwork.Parse(IPAddress, IPSubnetMask);
            this.IPAddress = IPAddress;

            if (Running)
            {
                OnChanged(this);
            }
        }

        public bool IsReachable(IPAddress IPAddress)
        {
            return IPNetwork.Contains(IPAddress) && !Equals(this.IPAddress, IPAddress);
        }

        public override string ToString()
        {
            if (IPNetwork == null)
            {
                return FriendlyName + " (no ip)";
            }

            return FriendlyName + " (" + IPAddress.ToString() + "/" + IPNetwork.SubnetMask.CIDR.ToString() + ")";
        }

        // Services

        private static List<InterfaceService> AvailableServices = new List<InterfaceService>
        {
            new RoutingService(),
            new ARP.ARPService(),
            new RIP.RIPService(),
            new LLDP.LLDPService(),
            new DHCP.DHCPService(),
            new ICMPEchoReplyService()
        };

        private List<InterfaceService> RunningServices = new List<InterfaceService>();

        private void ServicesInitialize()
        {
            var Services = GetAvailableServices();
            foreach (var Service in Services)
            {
                if (Service.DefaultRunning)
                {
                    ServiceToggle(Service.Name);
                }
            }
        }

        public void ServiceToggle(string ServiceName)
        {
            var Service = AvailableServices.Find(Entry => Entry.Name == ServiceName);
            if (Service == null)
            {
                throw new Exception("Service " + ServiceName + " does not exist.");
            }

            if (!ServiceRunning(ServiceName))
            {
                RunningServices.Add(Service);

                if (Running || !Service.OnlyRunningInterface)
                {
                    Service.OnStarted(this);
                }

                if (Service.OnlyRunningInterface)
                {
                    OnStarted += Service.OnStarted;
                    OnStopped += Service.OnStopped;
                }

                OnChanged += Service.OnChanged;
                OnPacketArrival += Service.OnPacketArrival;
            }
            else
            {
                RunningServices.Remove(Service);

                if (Service.OnlyRunningInterface)
                {
                    OnStarted -= Service.OnStarted;
                    OnStopped -= Service.OnStopped;
                }

                OnChanged -= Service.OnChanged;
                OnPacketArrival -= Service.OnPacketArrival;

                if (Running || !Service.OnlyRunningInterface)
                {
                    Service.OnStopped(this);
                }
            }
        }
 
        public bool ServiceExists(string SerivceName)
        {
            return AvailableServices.Exists(Entry => Entry.Name == SerivceName);
        }

        public bool ServiceRunning(string SerivceName)
        {
            return RunningServices.Exists(Entry => Entry.Name == SerivceName);
        }

        public List<InterfaceService> GetRunningServices()
        {
            return RunningServices.ToList();
        }

        public static List<InterfaceService> GetAvailableServices()
        {
            return AvailableServices.ToList();
        }

        // Equality

        public override int GetHashCode() => Name.GetHashCode();

        public bool Equals(Interface Interface) => !(Interface is null) && Equals(Interface.Name, Name);

        public override bool Equals(object obj) => !(obj is null) && obj.GetType() == GetType() && Equals(obj as Interface);
    }

    interface InterfaceService
    {
        string Name { get; }

        string Description { get; }

        bool OnlyRunningInterface { get; }

        bool DefaultRunning { get; }

        bool Anonymous { get; }

        void OnStarted(Interface Interface);

        void OnStopped(Interface Interface);

        void OnChanged(Interface Interface);

        void OnPacketArrival(Handler Handler);
    }
}