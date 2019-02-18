using System;
using System.Collections.Generic;
using System.Net;

namespace Router.DHCP
{
    class DHCPPool
    {
        public static Dictionary<Interface, DHCPPool> Interfaces { get; set; } = new Dictionary<Interface, DHCPPool>();

        private uint _firstIP;
        private uint _lastIP;

        public IPAddress FirstIP => UintToIp(_firstIP);
        public IPAddress LastIP => UintToIp(_lastIP);

        public int Available => (int)(_lastIP - _firstIP) + 1;
        public int Allocated => AllocatedIPs.Count;

        public bool IsDynamic { get; set; } = true;

        private List<uint> AllocatedIPs = new List<uint>();

        public DHCPPool(IPAddress FirstIP, IPAddress LastIP)
        {
            _firstIP = IpToUint(FirstIP);
            _lastIP = IpToUint(LastIP);

            if (_firstIP > _lastIP)
            {
                throw new Exception("FirstIP can't be greater than LastIP.");
            }
        }

        public IPAddress Allocate()
        {
            for (var IP = _firstIP; IP <= _lastIP; IP++)
            {
                if (AllocatedIPs.Contains(IP))
                {
                    continue;
                }

                AllocatedIPs.Add(IP);
                return UintToIp(IP);
            }

            return null;
        }

        public void Free(IPAddress IPAddress)
        {
            var IP = IpToUint(IPAddress);
            AllocatedIPs.Remove(IP);
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
