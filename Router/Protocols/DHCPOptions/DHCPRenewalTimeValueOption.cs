using System;

namespace Router.Protocols.DHCPOptions
{
    class DHCPRenewalTimeValueOption : DHCPUIntOption
    {
        public TimeSpan TimeSpan => TimeSpan.FromSeconds(Value);

        public DHCPRenewalTimeValueOption(byte[] Bytes) : base(DHCPOptionCode.RenewalTimeValue, Bytes) { }

        public DHCPRenewalTimeValueOption(uint RenewalTime) : base(DHCPOptionCode.RenewalTimeValue, RenewalTime) { }

        public DHCPRenewalTimeValueOption(TimeSpan RenewalTime) : base(DHCPOptionCode.RenewalTimeValue, (uint)RenewalTime.Seconds) { }
    }
}
