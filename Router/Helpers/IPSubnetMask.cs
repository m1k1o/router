using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.Helpers
{
    public class IPSubnetMask : IPAddress
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

        public static int SubnetMaskToCIDR(IPSubnetMask SubnetMask)
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

        public static IPSubnetMask CIDRToSubnetMask(int CIDR)
        {
            throw new NotImplementedException();
        }

        public static bool IsValidSubnetMask(IPSubnetMask SubnetMask)
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
    }
}
