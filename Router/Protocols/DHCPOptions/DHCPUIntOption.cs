using System;

namespace Router.Protocols.DHCPOptions
{
    abstract class DHCPUIntOption : DHCPOption
    {
        private uint Value;

        public DHCPUIntOption(DHCPOptionCode DHCPOptionCode, byte[] Bytes) : base(DHCPOptionCode)
        {
            // TODO: Bad Practices
            Value = BitConverter.ToUInt32(new Byte[4] { Bytes[3], Bytes[2], Bytes[1], Bytes[0] }, 0);
        }

        public DHCPUIntOption(DHCPOptionCode DHCPOptionCode, uint Value) : base(DHCPOptionCode)
        {
            this.Value = Value;
        }

        public override byte[] Bytes
        {
            get
            {
                // TODO: Bad Practices
                var RawData = new byte[4];
                var Bytes = BitConverter.GetBytes(Value);
                RawData[3] = Bytes[0];
                RawData[2] = Bytes[1];
                RawData[1] = Bytes[2];
                RawData[0] = Bytes[3];
                return RawData;
            }
        }
    }
}
