using Router.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Router.RIP
{
    public sealed class RIPPacket : Packet
    {
        public byte CommandType
        {
            get => (byte)Slice(0, typeof(byte));
            set => Inject(0, value);
        }

        public byte Version
        {
            get => (byte)Slice(1, typeof(byte));
            set => Inject(1, value);
        }

        public ushort MustBeZero
        {
            get => (ushort)Slice(2, typeof(ushort));
            set => Inject(2, value);
        }

        public RTECollection RTEs
        {
            get
            {
                var offset = 4;
                var newColection = new RTECollection();
                do {
                    var RTEBytes = Slice(offset, RTE.Length);
                    newColection.Add(new RTE(RTEBytes));
                    offset += RTE.Length;
                } while (offset < Length);

                return newColection;
            }

            set
            {
                var offset = 4;
                foreach (RTE item in value)
                {
                    Inject(offset, item.Bytes, item.Bytes.Length);
                    offset += item.Bytes.Length;
                }
            }
        }

        public RIPPacket(RIPCommandType CommandType, RTECollection RTEs) : base(4)
        {
            this.CommandType = (byte)CommandType;
            Version = 2;
            MustBeZero = 0;
            this.RTEs = RTEs;
        }

        public RIPPacket(byte[] Data) : base(Data)
        {

        }
        public override string ToString()
        {
            return "RIP " + CommandType + "\n" + RTEs.ToString();
        }
    }
}