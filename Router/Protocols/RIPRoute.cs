using Router.Helpers;
using System.Net;

namespace Router.Protocols
{
    public class RIPRoute : Packet
    {
        public new const int Length = 20;

        public ushort AddressFamilyIdentifier
        {
            get => (ushort)Slice(0, typeof(ushort));
            set => Inject(0, value);
        }

        public ushort RouteTag
        {
            get => (ushort)Slice(2, typeof(ushort));
            set => Inject(2, value);
        }

        public IPAddress IPAddress
        {
            get => (IPAddress)Slice(4, typeof(IPAddress));
            set => Inject(4, value);
        }

        public IPSubnetMask IPSubnetMask
        {
            get => (IPSubnetMask)Slice(8, typeof(IPSubnetMask));
            set => Inject(8, value);
        }

        public IPAddress NextHop
        {
            get => (IPAddress)Slice(12, typeof(IPAddress));
            set => Inject(12, value);
        }

        public uint Metric
        {
            get => (uint)Slice(16, typeof(uint));
            set => Inject(16, value);
        }

        public RIPRoute(IPAddress IPAddress, IPSubnetMask IPSubnetMask, IPAddress NextHop, uint Metric) : base(20)
        {
            AddressFamilyIdentifier = 2;
            RouteTag = 0;
            this.IPAddress = IPAddress;
            this.IPSubnetMask = IPSubnetMask;
            this.NextHop = NextHop;
            this.Metric = Metric;
        }

        public RIPRoute(byte[] Data) : base(Data)
        {

        }

        public override string ToString()
        {
            return
                "AddressFamily:\t" + AddressFamilyIdentifier.ToString() + "\n" +
                "RouteTag:\t" + RouteTag.ToString() + "\n" +
                "IPAddress:\t" + IPAddress.ToString() + "\n" +
                "SubnetMask:\t" + IPSubnetMask.ToString() + "\n" +
                "NextHop:\t" + NextHop.ToString() + "\n" +
                "IPAddress:\t" + IPAddress.ToString() + "\n" +
                "Metric:\t" + Metric.ToString();
        }
    }
}
