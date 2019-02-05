using System;
using System.Collections.Generic;

namespace Router.RIP
{
    public sealed class RIPPacket
    {
        private byte[] Data = new Byte[24];

        public byte CommandType
        {
            get
            {
                return Data[0];
            }

            set
            {
                Data[0] = (byte)value;
            }
        }

        public byte Version
        {
            get
            {
                return Data[1];
            }

            set
            {
                Data[1] = (byte)value;
            }
        }

        public ushort MustBeZero
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

        public List<RTE> RTEs
        {
            get
            {
                var bytes = Data.Length;
                var routes = (bytes - 4) / 20;
                Console.Write(routes.ToString());

                var ret = new List<RTE>();
                for (int i = 0; i < routes; i++)
                {
                    byte[] Dst = new Byte[20];
                    Array.Copy(Data, 4 + i * 5, Dst, 0, 20);
                    ret.Add(new RTE(Dst));
                }

                return ret;
            }

            set
            {
                int newSize = 4 + (value.Count * 20);
                if(newSize != Data.Length)
                {
                    Array.Resize(ref Data, 4 + (value.Count * 20));
                }

                int offset = 4;
                foreach (RTE item in value)
                {
                    
                    byte[] Src = item.Export();
                    Array.Copy(Data, offset, Src, 0, 20);
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

        public RIPPacket(byte[] Data)
        {
            Array.Copy(Data, 0, this.Data, 0, Data.Length);
        }

        public byte[] Export()
        {
            return Data;
        }
    }
}