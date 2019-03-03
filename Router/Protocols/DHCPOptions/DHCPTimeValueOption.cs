using System;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPTimeValueOption : DHCPOption
    {
        public TimeSpan TimeSpan
        {
            get => TimeSpan.FromSeconds(Seconds);
            set
            {
                Seconds = (uint)value.TotalSeconds;
            }
        }

        public uint Seconds { get; set; }

        public DHCPTimeValueOption(DHCPOptionCode DHCPOptionCode) : base(DHCPOptionCode)
        {
            Seconds = 0;
        }

        public DHCPTimeValueOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            // TODO: Bad Practices
            Seconds = BitConverter.ToUInt32(new Byte[4] { Bytes[3], Bytes[2], Bytes[1], Bytes[0] }, 0);
        }

        public DHCPTimeValueOption(DHCPOptionCode DHCPOptionCode, uint Value) : base(DHCPOptionCode)
        {
            this.Seconds = Value;
        }

        public override byte[] Bytes
        {
            get
            {
                // TODO: Bad Practices
                var RawData = new byte[4];
                var Bytes = BitConverter.GetBytes(Seconds);
                RawData[3] = Bytes[0];
                RawData[2] = Bytes[1];
                RawData[1] = Bytes[2];
                RawData[0] = Bytes[3];
                return RawData;
            }
        }
    }
}
