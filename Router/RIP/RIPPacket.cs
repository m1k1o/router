using System;
using System.Collections.Generic;

namespace Router.RIP
{
    public sealed class RIPPacket
    {
        public byte[] Bytes { get; private set; } = new Byte[24];

        public byte CommandType
        {
            get
            {
                return Bytes[0];
            }

            set
            {
                Bytes[0] = (byte)value;
            }
        }

        public byte Version
        {
            get
            {
                return Bytes[1];
            }

            set
            {
                Bytes[1] = (byte)value;
            }
        }

        public ushort MustBeZero
        {
            get
            {
                var len = 2;
                var offset = 2;
                byte[] Dst = new Byte[len];
                Array.Copy(Bytes, offset, Dst, 0, len);
                return BitConverter.ToUInt16(Dst, 0);
            }

            set
            {
                var len = 2;
                var offset = 2;
                byte[] Src = BitConverter.GetBytes(value);
                Array.Copy(Src, 0, Bytes, offset, len);
            }
        }

        public List<RTE> RTEs
        {
            get
            {
                var bytes = Bytes.Length;
                var routes = (bytes - 4) / 20;
                Console.Write(routes.ToString());

                var ret = new List<RTE>();
                for (int i = 0; i < routes; i++)
                {
                    byte[] Dst = new Byte[20];
                    Array.Copy(Bytes, 4 + i * 5, Dst, 0, 20);
                    ret.Add(new RTE(Dst));
                }

                return ret;
            }

            set
            {
                int newSize = 4 + (value.Count * 20);
                if(newSize != Bytes.Length)
                {
                    Array.Resize(ref Data, 4 + (value.Count * 20));
                }

                int offset = 4;
                foreach (RTE item in value)
                {

                    byte[] Src = item.Bytes;
                    Array.Copy(Bytes, offset, Src, 0, 20);
                    offset += 20;
                }
            }
        }
        
        public RIPPacket(RIPCommandType CommandType, List<RTE> RTEs)
        {
            this.CommandType = (byte)CommandType;
            Version = 2;
            MustBeZero = 0;
            this.RTEs = RTEs;
        }

        public RIPPacket(byte[] Bytes)
        {
            Array.Copy(Bytes, 0, this.Bytes, 0, Bytes.Length);
        }
    }
}