using System;

namespace Router.Protocols.DHCPOptions
{
    class DHCPRenewalTimeValueOption : DHCPTimeValueOption
    {
        public DHCPRenewalTimeValueOption() : base(DHCPOptionCode.RenewalTimeValue) { }

        public DHCPRenewalTimeValueOption(byte[] Bytes) : base(DHCPOptionCode.RenewalTimeValue, Bytes) { }

        public DHCPRenewalTimeValueOption(uint RenewalTime) : base(DHCPOptionCode.RenewalTimeValue, RenewalTime) { }

        public DHCPRenewalTimeValueOption(TimeSpan RenewalTime) : base(DHCPOptionCode.RenewalTimeValue, (uint)RenewalTime.TotalSeconds) { }
    }
}
