using System;

namespace Router.Protocols.DHCPOptions
{
    class DHCPRebindingTimeValueOption : DHCPTimeValueOption
    {
        public DHCPRebindingTimeValueOption() : base(DHCPOptionCode.RebindingTimeValue) { }

        public DHCPRebindingTimeValueOption(byte[] Bytes) : base(DHCPOptionCode.RebindingTimeValue, Bytes) { }

        public DHCPRebindingTimeValueOption(uint RebindingTime) : base(DHCPOptionCode.RebindingTimeValue, RebindingTime) { }

        public DHCPRebindingTimeValueOption(TimeSpan RebindingTime) : base(DHCPOptionCode.RebindingTimeValue, (uint)RebindingTime.TotalSeconds) { }
    }
}
