using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Router.Helpers
{
    abstract class Packet
    {
        private byte[] RawData;

        public int Length { get => RawData.Length; }

        public byte[] Bytes { get => RawData; }

        protected Packet(int ArraySize)
        {
            RawData = new Byte[ArraySize];
        }

        public Packet(byte[] Data)
        {
            RawData = Data;
        }

        protected object Slice(int o, Type Type)
        {
            if (Type == typeof(byte))
            {
                return RawData[o];
            }

            if (Type == typeof(ushort))
            {
                return BitConverter.ToUInt16(new Byte[2] { RawData[o + 1], RawData[o] }, 0);
            }

            if (Type == typeof(uint))
            {
                return BitConverter.ToUInt32(new Byte[4] { RawData[o + 3], RawData[o + 2], RawData[o + 1], RawData[o] }, 0);
            }

            if (Type == typeof(IPAddress))
            {
                byte[] Dst = new Byte[4];
                Array.Copy(RawData, o, Dst, 0, 4);
                return new IPAddress(Dst);
            }

            if (Type == typeof(IPSubnetMask))
            {
                byte[] Dst = new Byte[4];
                Array.Copy(RawData, o, Dst, 0, 4);
                return new IPSubnetMask(Dst);
            }

            if (Type == typeof(PhysicalAddress))
            {
                byte[] Dst = new Byte[6];
                Array.Copy(RawData, o, Dst, 0, 6);
                return new PhysicalAddress(Dst);
            }
            
            return null;
        }

        protected void Inject(int o, object Value)
        {
            if (Value is byte)
            {
                RawData[o] = (byte)Value;
                return;
            }

            if (Value is ushort)
            {
                var Bytes = BitConverter.GetBytes((ushort)Value);
                RawData[o + 1] = Bytes[0];
                RawData[o] = Bytes[1];
                return;
            }

            if (Value is uint)
            {
                var Bytes = BitConverter.GetBytes((uint)Value);
                RawData[o + 3] = Bytes[0];
                RawData[o + 2] = Bytes[1];
                RawData[o + 1] = Bytes[2];
                RawData[o] = Bytes[3];
                return;
            }

            if (Value is IPAddress)
            {
                var IPBytes = ((IPAddress)Value).GetAddressBytes();
                Array.Copy(IPBytes, 0, RawData, o, IPBytes.Length);
                return;
            }

            if (Value is IPSubnetMask)
            {
                var IPBytes = ((IPSubnetMask)Value).GetAddressBytes();
                Array.Copy(IPBytes, 0, RawData, o, IPBytes.Length);
                return;
            }

            if (Value is PhysicalAddress)
            {
                var IPBytes = ((PhysicalAddress)Value).GetAddressBytes();
                Array.Copy(IPBytes, 0, RawData, o, IPBytes.Length);
                return;
            }
        }

        protected byte[] Slice(int o, int len)
        {
            var Value = new Byte[len];
            for (var i = 0; i < len && i < Length; i++)
            {
                Value[i] = RawData[o + i];
            }

            return Value;
        }

        protected void Inject(int o, byte[] Value, int len)
        {
            Expand(o + len);

            for (var i = 0; i < len; i++)
            {
                RawData[o + i] = Value[i];
            }
        }

        protected void Expand(int len)
        {
            if (len > Length)
            {
                Array.Resize(ref RawData, len);
            }
        }
    }
}
