using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.Helpers
{
    class IPNetwork
    {
        public IPAddress NetworkAddress { get; private set; }

        public IPAddress BroadcastAddress { get; private set; }

        public IPAddress SubnetMask { get; private set; }

        public int CIDR { get; private set; }

        IPNetwork(IPAddress Address, IPAddress SubnetMask)
        {
            if (!IsValidSubnetMask(SubnetMask))
            {
                throw new Exception("Given SubnetMask is not valid.");
            }

            NetworkAddress = GetNetworkAddress(Address, SubnetMask);
            BroadcastAddress = GetBroadcastAddress(Address, SubnetMask);
            CIDR = SubnetMaskToCIDR(SubnetMask);
            this.SubnetMask = SubnetMask;
        }

        public bool Contains(IPAddress IPAddress)
        {
            IPAddress Network = GetNetworkAddress(IPAddress, SubnetMask);
            return Network.Equals(NetworkAddress);
        }

        public override string ToString()
        {
            return NetworkAddress.ToString() + "/" + CIDR.ToString();
        }

        public bool Equals(IPNetwork IPNetwork)
        {
            if (IPNetwork is null)
            {
                return false;
            }

            if (ReferenceEquals(this, IPNetwork))
            {
                return true;
            }

            return Equals(IPNetwork.NetworkAddress, NetworkAddress) && IPNetwork.CIDR == CIDR;
        }
        
        public override bool Equals(object obj)
        {
            return obj.GetType() == GetType() && Equals(obj as IPNetwork);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = NetworkAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ SubnetMask.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(IPNetwork obj1, IPNetwork obj2)
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

        public static bool operator !=(IPNetwork obj1, IPNetwork obj2)
        {
            return !(obj1 == obj2);
        }

        public static bool operator <(IPNetwork obj1, IPNetwork obj2)
        {
            return Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.CIDR < obj2.CIDR;
        }

        public static bool operator >(IPNetwork obj1, IPNetwork obj2)
        {
            return Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.CIDR > obj2.CIDR;
        }

        public static bool operator <=(IPNetwork obj1, IPNetwork obj2)
        {
            return Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.CIDR <= obj2.CIDR;
        }

        public static bool operator >=(IPNetwork obj1, IPNetwork obj2)
        {
            return Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.CIDR >= obj2.CIDR;
        }

        public static IPNetwork Parse(IPAddress Address, IPAddress SubnetMask)
        {
            return new IPNetwork(Address, SubnetMask);
        }

        public static IPNetwork Parse(string Address, string SubnetMask)
        {
            return new IPNetwork(IPAddress.Parse(Address), IPAddress.Parse(SubnetMask));
        }

        public static IPAddress GetBroadcastAddress(IPAddress Address, IPAddress SubnetMask)
        {
            byte[] ipAdressBytes = Address.GetAddressBytes();
            byte[] SubnetMaskBytes = SubnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != SubnetMaskBytes.Length)
            {
                throw new ArgumentException("Lengths of IP Address and Subnet mask do not match.");
            }

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (SubnetMaskBytes[i] ^ 255));
            }

            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(IPAddress Address, IPAddress SubnetMask)
        {
            byte[] ipAdressBytes = Address.GetAddressBytes();
            byte[] SubnetMaskBytes = SubnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != SubnetMaskBytes.Length)
            {
                throw new ArgumentException("Lengths of IP Address and Subnet mask do not match.");
            }

            byte[] networkAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < networkAddress.Length; i++)
            {
                networkAddress[i] = (byte)(ipAdressBytes[i] & (SubnetMaskBytes[i]));
            }

            return new IPAddress(networkAddress);
        }

        public static int SubnetMaskToCIDR(IPAddress SubnetMask)
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

        public static bool IsValidSubnetMask(IPAddress SubnetMask)
        {
            var value = BitConverter.ToUInt32(SubnetMask.GetAddressBytes(), 0);
            if(value == 0)
            {
                return true;
            }

            uint total = UInt32.MaxValue;
            while (total != 0)
            {
                var ip = (uint) IPAddress.HostToNetworkOrder((int) total);

                if (ip == value)
                {
                    return true;
                }

                total = (total << 1);
            }

            return false;
        }
    }
}
