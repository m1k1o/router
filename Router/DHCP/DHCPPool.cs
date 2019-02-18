using System;
using System.Collections.Generic;
using System.Net;

namespace Router.DHCP
{
    class DHCPPool
    {
        public static Dictionary<Interface, DHCPPool> Interfaces { get; set; } = new Dictionary<Interface, DHCPPool>();

        public uint FirstIP { get; private set; }
        public uint LastIP { get; private set; }

        private List<uint> UsedIPs = new List<uint>();

        public DHCPPool(IPAddress FirstIP, IPAddress LastIP)
        {
            this.FirstIP = IpToUint(FirstIP);
            this.LastIP = IpToUint(LastIP);

            if (this.FirstIP > this.LastIP)
            {
                throw new Exception("FirstIP can't be greater than LastIP.");
            }
        }

        public IPAddress Allocate()
        {
            for (var IP = FirstIP; IP <= LastIP; IP++)
            {
                if (UsedIPs.Contains(IP))
                {
                    continue;
                }

                UsedIPs.Add(IP);
                return UintToIp(IP);
            }

            return null;
        }

        public void Free(IPAddress IPAddress)
        {
            var IP = IpToUint(IPAddress);
            UsedIPs.Remove(IP);
        }

        public static uint IpToUint(IPAddress IPAddress)
        {
            byte[] Bytes = IPAddress.GetAddressBytes();
            Array.Reverse(Bytes); // flip big-endian(network order) to little-endian
            return BitConverter.ToUInt32(Bytes, 0);
        }

        public static IPAddress UintToIp(uint Number)
        {
            byte[] Bytes = BitConverter.GetBytes(Number);
            Array.Reverse(Bytes); // flip little-endian to big-endian(network order)
            return new IPAddress(Bytes);
        }
    }
}
