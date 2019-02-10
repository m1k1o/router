using System;
using System.Net;
using Router.Helpers;
using SharpPcap;

namespace Router
{
    delegate void InterfaceEvent(Interface Interface);

    class Interface : Device
    {
        public int ID { get; private set; }

        public InterfaceEvent OnStarted { get; set; } = new InterfaceEvent(I => { });
        public InterfaceEvent OnStopped { get; set; } = new InterfaceEvent(I => { });

        public IPAddress IPAddress { get; private set; }
        public IPNetwork IPNetwork { get; private set; }

        public Interface(ICaptureDevice ICaptureDevice, int ID) : base(ICaptureDevice)
        {
            this.ID = ID;

            OnPacketArrival = (Packet) => 
            {
                Interfaces.OnPacketArrival(Packet, this);
            };

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

            AfterStarted = () =>
            {
                RoutingTable.Instance.PushDirectlyConnected(this, IPNetwork);
                OnStarted(this);
            };

            AfterStopped = () =>
            {
                try
                {
                    OnStopped(this);
                }
                finally
                {
                    RoutingTable.Instance.RemoveDirectlyConnected(this);
                }
            };
        }

        public void SetIP(IPAddress IPAddress, IPSubnetMask IPSubnetMask)
        {
            IPNetwork = IPNetwork.Parse(IPAddress, IPSubnetMask);
            this.IPAddress = IPAddress;
        }

        public bool IsReachable(IPAddress IPAddress)
        {
            return IPNetwork.Contains(IPAddress) && !Equals(this.IPAddress, IPAddress);
        }

        public override string ToString()
        {
            if (IPNetwork == null)
            {
                return FriendlyName + "(no ip)";
            }

            return FriendlyName + "(" + IPAddress.ToString() + "/" + IPNetwork.SubnetMask.CIDR.ToString() + ")";
        }

        // Equality

        public override int GetHashCode() => Name.GetHashCode();

        public bool Equals(Interface Interface) => !(Interface is null) && Equals(Interface.Name, Name);

        public override bool Equals(object obj) => !(obj is null) && obj.GetType() == GetType() && Equals(obj as Interface);
    }
}