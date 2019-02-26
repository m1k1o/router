using Router.LLDP;
using System;

namespace Router.Controllers.LLDP
{
    class Settings : Controller
    {
        public double AdvInterval
        {
            get => LLDPAdvertisements.Timer.TotalSeconds;
            set => LLDPAdvertisements.Timer = TimeSpan.FromSeconds(value);
        }

        public ushort TimeToLive
        {
            get => LLDPResponse.TimeToLive;
            set => LLDPResponse.TimeToLive = value;
        }

        public string SystemName
        {
            get => LLDPResponse.SystemName;
            set => LLDPResponse.SystemName = value;
        }

        public string SystemDescription
        {
            get => LLDPResponse.SystemDescription;
            set => LLDPResponse.SystemDescription = value;
        }

        public object Export() => this;
    }
}
