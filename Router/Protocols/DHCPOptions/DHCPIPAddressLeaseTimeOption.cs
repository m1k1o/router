using System;

namespace Router.Protocols.DHCPOptions
{
    class DHCPIPAddressLeaseTimeOption : DHCPOption
    {
        private uint LeaseTime;

        public DHCPIPAddressLeaseTimeOption(byte[] Bytes) : base(DHCPOptionCode.IPAddressLeaseTime)
        {
            // TODO: Bad Practices
            LeaseTime = BitConverter.ToUInt32(new Byte[4] { Bytes[3], Bytes[2], Bytes[1], Bytes[0] }, 0);
        }

        public DHCPIPAddressLeaseTimeOption(uint LeaseTime) : base(DHCPOptionCode.IPAddressLeaseTime)
        {
            this.LeaseTime = LeaseTime;
        }

        public DHCPIPAddressLeaseTimeOption(TimeSpan LeaseTime) : base(DHCPOptionCode.IPAddressLeaseTime)
        {
            this.LeaseTime = (uint)LeaseTime.Seconds;
        }

        public override byte[] Bytes
        {
            get
            {
                // TODO: Bad Practices
                var RawData = new byte[4];
                var Bytes = BitConverter.GetBytes(LeaseTime);
                RawData[3] = Bytes[0];
                RawData[2] = Bytes[1];
                RawData[1] = Bytes[2];
                RawData[0] = Bytes[3];
                return RawData;
            }
        }
    }
}
