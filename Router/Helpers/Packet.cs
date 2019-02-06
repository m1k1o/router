using System;
using System.Net;

namespace Router.Helpers
{
    public abstract class Packet
    {
        protected byte[] Data;

        public int Length { get => Data.Length; }

        public byte[] Bytes { get => Data; }

        protected Packet(int ArraySize)
        {
            Data = new Byte[ArraySize];
        }

        public Packet(byte[] Data)
        {
            Array.Copy(Data, 0, this.Data, 0, Data.Length);
        }

        protected object Slice(int o, Type Type)
        {
            if (Type == typeof(byte))
            {
                return (byte)Data[o];
            }

            if (Type == typeof(ushort))
            {
                return (ushort)BitConverter.ToUInt16(new Byte[2] { Data[o + 1], Data[o] }, 0);
            }

            if (Type == typeof(uint))
            {
                return (uint)BitConverter.ToUInt32(new Byte[4] { Data[o + 3], Data[o + 2], Data[o + 1], Data[o] }, 0);
            }

            if (Type == typeof(IPAddress))
            {
                byte[] Dst = new Byte[4];
                Array.Copy(Data, o, Dst, 0, 4);
                return (IPAddress)new IPAddress(Dst);
            }

            return null;
        }

        protected void Inject(int o, object Value)
        {
            if (Value is byte)
            {
                Data[o] = (byte)Value;
                return;
            }

            if (Value is ushort)
            {
                var Bytes = BitConverter.GetBytes((ushort)Value);
                Data[o + 1] = Bytes[0];
                Data[o] = Bytes[1];
                return;
            }

            if (Value is uint)
            {
                var Bytes = BitConverter.GetBytes((uint)Value);
                Data[o + 3] = Bytes[0];
                Data[o + 2] = Bytes[1];
                Data[o + 1] = Bytes[2];
                Data[o] = Bytes[3];
                return;
            }

            if (Value is IPAddress)
            {
                var IPBytes = ((IPAddress)Value).GetAddressBytes();
                Array.Copy(IPBytes, 0, Data, o, IPBytes.Length);
                return;
            }
        }

        protected byte[] Slice(int o, int len)
        {
            var Value = new Byte[len];
            for (var i = 0; i < len && i < Length; i++)
            {
                Value[i] = Data[o + i];
            }

            return Value;
        }

        protected void Inject(int o, byte[] Value, int len)
        {
            Expand(o + len);

            for (var i = 0; i < len; i++)
            {
                Data[o + i] = Value[i];
            }
        }

        protected void Expand(int len)
        {
            if (len > Length)
            {
                Array.Resize(ref Data, len);
            }
        }
    }
}
