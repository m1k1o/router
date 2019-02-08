using System;
using System.Net;

namespace Router.Helpers
{
    class IPSubnetMask : IPAddress
    {
        public int CIDR { get; private set; }

        private void Verify()
        {
            if (!IsValidSubnetMask(this))
            {
                throw new Exception("Given mask is not valid.");
            }

            CIDR = SubnetMaskToCIDR(this);
        }

        public IPSubnetMask(long newAddress) : base(newAddress)
        {
            Verify();
        }

        public IPSubnetMask(byte[] address) : base(address)
        {
            Verify();
        }

        public IPSubnetMask(byte[] address, long scopeid) : base(address, scopeid)
        {
            Verify();
        }

        static public int SubnetMaskToCIDR(IPSubnetMask SubnetMask)
        {
            var value = BitConverter.ToUInt32(SubnetMask.GetAddressBytes(), 0);

            int count = 0;
            while (value != 0)
            {
                count++;
                value &= value - 1;
            }

            return count;
        }

        static public IPSubnetMask CIDRToSubnetMask(int CIDR)
        {
            throw new NotImplementedException();
        }

        static public bool IsValidSubnetMask(IPSubnetMask SubnetMask)
        {
            var value = BitConverter.ToUInt32(SubnetMask.GetAddressBytes(), 0);
            if (value == 0)
            {
                return true;
            }

            uint total = UInt32.MaxValue;
            while (total != 0)
            {
                var ip = (uint)IPAddress.HostToNetworkOrder((int)total);

                if (ip == value)
                {
                    return true;
                }

                total = (total << 1);
            }

            return false;
        }

        static new public IPSubnetMask Parse(string ipSubnetString)
        {
            return new IPSubnetMask(IPAddress.Parse(ipSubnetString).GetAddressBytes());
        }

        // Equality

        public override int GetHashCode()=> CIDR.GetHashCode();

        public bool Equals(IPSubnetMask IPSubnetMask) => !(IPSubnetMask is null) && IPSubnetMask.CIDR == CIDR;

        public override bool Equals(object obj) => !(obj is null) && obj.GetType() == GetType() && Equals(obj as IPSubnetMask);

        static public bool operator ==(IPSubnetMask obj1, IPSubnetMask obj2) => Equals(obj1, obj2);

        public static bool operator !=(IPSubnetMask obj1, IPSubnetMask obj2) => !(obj1 == obj2);

        public static bool operator <(IPSubnetMask obj1, IPSubnetMask obj2) => obj1.CIDR < obj2.CIDR;

        public static bool operator >(IPSubnetMask obj1, IPSubnetMask obj2) => obj1.CIDR > obj2.CIDR;

        public static bool operator <=(IPSubnetMask obj1, IPSubnetMask obj2) => obj1.CIDR <= obj2.CIDR;

        public static bool operator >=(IPSubnetMask obj1, IPSubnetMask obj2) => obj1.CIDR >= obj2.CIDR;
    }
}
