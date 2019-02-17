using System;

namespace Router.Protocols.DHCPOptions
{
    class DHCPRebindingTimeValueOption : DHCPUIntOption
    {
        public DHCPRebindingTimeValueOption(byte[] Bytes) : base(DHCPOptionCode.RebindingTimeValue, Bytes) { }

        public DHCPRebindingTimeValueOption(uint RebindingTime) : base(DHCPOptionCode.RebindingTimeValue, RebindingTime) { }

        public DHCPRebindingTimeValueOption(TimeSpan RebindingTime) : base(DHCPOptionCode.RebindingTimeValue, (uint)RebindingTime.Seconds) { }
    }
}
