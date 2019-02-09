using System;
using System.Net;

namespace Router.Helpers
{
    class IPNetwork
    {
        public IPAddress NetworkAddress { get; private set; }
        public IPAddress BroadcastAddress { get; private set; }
        public IPSubnetMask SubnetMask { get; private set; }

        private IPNetwork(IPAddress Address, IPSubnetMask SubnetMask)
        {
            NetworkAddress = GetNetworkAddress(Address, SubnetMask);
            BroadcastAddress = GetBroadcastAddress(Address, SubnetMask);
            this.SubnetMask = SubnetMask;
        }

        public bool Contains(IPAddress IPAddress)
        {
            IPAddress Network = GetNetworkAddress(IPAddress, SubnetMask);
            return Equals(Network, NetworkAddress);
        }

        public override string ToString()
        {
            return NetworkAddress.ToString() + "/" + SubnetMask.CIDR;
        }

        public static IPNetwork Parse(IPAddress Address, IPSubnetMask SubnetMask)
        {
            return new IPNetwork(Address, SubnetMask);
        }

        public static IPNetwork Parse(string Address, string SubnetMask)
        {
            return new IPNetwork(IPAddress.Parse(Address), IPSubnetMask.Parse(SubnetMask));
        }

        public static IPAddress GetBroadcastAddress(IPAddress Address, IPSubnetMask SubnetMask)
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

        public static IPAddress GetNetworkAddress(IPAddress Address, IPSubnetMask SubnetMask)
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

        // Equality

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = NetworkAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ SubnetMask.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(IPNetwork IPNetwork)
            => !(IPNetwork is null) && Equals(IPNetwork.NetworkAddress, NetworkAddress) && IPNetwork.SubnetMask == SubnetMask;

        public override bool Equals(object obj)
            => !(obj is null) && obj.GetType() == GetType() && Equals(obj as IPNetwork);

        public static bool operator ==(IPNetwork obj1, IPNetwork obj2)
            => Equals(obj1, obj2);

        public static bool operator !=(IPNetwork obj1, IPNetwork obj2)
            => !(obj1 == obj2);

        public static bool operator <(IPNetwork obj1, IPNetwork obj2)
            => Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.SubnetMask < obj2.SubnetMask;

        public static bool operator >(IPNetwork obj1, IPNetwork obj2)
            => Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.SubnetMask > obj2.SubnetMask;

        public static bool operator <=(IPNetwork obj1, IPNetwork obj2)
            => Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.SubnetMask <= obj2.SubnetMask;

        public static bool operator >=(IPNetwork obj1, IPNetwork obj2)
            => Equals(obj1.NetworkAddress, obj2.NetworkAddress) && obj1.SubnetMask >= obj2.SubnetMask;
    }
}
