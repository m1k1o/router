using System;
using System.Net;

namespace Router.RIP
{
    public class RTE
    {
        private byte[] Data = new Byte[20];

        public byte[] Bytes { get => Data; }

        public ushort AddressFamilyIdentifier
        {
            get
            {
                var len = 2;
                var offset = 0;
                byte[] Dst = new Byte[len];
                Array.Copy(Data, offset, Dst, 0, len);
                return BitConverter.ToUInt16(Dst, 0);
            }

            set
            {
                var len = 2;
                var offset = 0;
                byte[] Src = BitConverter.GetBytes(value);
                Array.Copy(Src, 0, Data, offset, len);
            }
        }

        public ushort RouteTag
        {
            get
            {
                var len = 2;
                var offset = 2;
                byte[] Dst = new Byte[len];
                Array.Copy(Data, offset, Dst, 0, len);
                return BitConverter.ToUInt16(Dst, 0);
            }

            set
            {
                var len = 2;
                var offset = 2;
                byte[] Src = BitConverter.GetBytes(value);
                Array.Copy(Src, 0, Data, offset, len);
            }
        }

        public IPAddress IPAddress
        {
            get
            {
                var len = 4;
                var offset = 4;
                byte[] Dst = new Byte[len];
                Array.Copy(Data, offset, Dst, 0, len);
                return new IPAddress(Dst);
            }

            set
            {
                var len = 4;
                var offset = 4;
                byte[] Src = value.GetAddressBytes();
                Array.Copy(Src, 0, Data, offset, len);
            }
        }

        public IPAddress SubnetMask
        {
            get
            {
                var len = 4;
                var offset = 8;
                byte[] Dst = new Byte[len];
                Array.Copy(Data, offset, Dst, 0, len);
                return new IPAddress(Dst);
            }

            set
            {
                var len = 4;
                var offset = 8;
                byte[] Src = value.GetAddressBytes();
                Array.Copy(Src, 0, Data, offset, len);
            }
        }

        public IPAddress NextHop
        {
            get
            {
                var len = 4;
                var offset = 12;
                byte[] Dst = new Byte[len];
                Array.Copy(Data, offset, Dst, 0, len);
                return new IPAddress(Dst);
            }

            set
            {
                var len = 4;
                var offset = 12;
                byte[] Src = value.GetAddressBytes();
                Array.Copy(Src, 0, Data, offset, len);
            }
        }

        public uint Metric
        {
            get
            {
                var len = 4;
                var offset = 16;
                byte[] Dst = new Byte[len];
                Array.Copy(Data, offset, Dst, 0, len);
                return BitConverter.ToUInt32(Dst, 0);
            }

            set
            {
                var len = 4;
                var offset = 16;
                byte[] Src = BitConverter.GetBytes(value);
                Array.Copy(Src, 0, Data, offset, len);
            }
        }

        public RTE(IPAddress IPAddress, IPAddress SubnetMask, IPAddress NextHop, uint Metric)
        {
            AddressFamilyIdentifier = 2;
            RouteTag = 0;
            this.IPAddress = IPAddress;
            this.SubnetMask = SubnetMask;
            this.NextHop = NextHop;
            this.Metric = Metric;
        }

        public RTE(byte[] Data)
        {
            Array.Copy(Data, 0, this.Data, 0, 20);
        }
    }
}