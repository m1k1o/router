using System;

namespace Router.Protocols.DHCPOptions
{
    class DHCPIPAddressLeaseTimeOption : DHCPUIntOption
    {
        public TimeSpan TimeSpan => TimeSpan.FromSeconds(Value);

        public DHCPIPAddressLeaseTimeOption(string String) : base(DHCPOptionCode.IPAddressLeaseTime, String) { }

        public DHCPIPAddressLeaseTimeOption(byte[] Bytes) : base(DHCPOptionCode.IPAddressLeaseTime, Bytes) { }

        public DHCPIPAddressLeaseTimeOption(uint LeaseTime) : base(DHCPOptionCode.IPAddressLeaseTime, LeaseTime) { }

        public DHCPIPAddressLeaseTimeOption(TimeSpan LeaseTime) : base(DHCPOptionCode.IPAddressLeaseTime, (uint)LeaseTime.TotalSeconds) { }

    }
}
