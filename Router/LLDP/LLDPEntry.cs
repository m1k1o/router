using PacketDotNet;
using PacketDotNet.LLDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.LLDP
{
    class LLDPEntry
    {
        public static TimeSpan CacheTimeout { get; set; } = TimeSpan.FromSeconds(128);

        public ChassisID ChassisID { get; private set; } = null;
        public PortID PortID { get; private set; } = null;
        public TimeToLive TimeToLive { get; private set; } = null;
        public PortDescription PortDescription { get; private set; } = null;
        public SystemName SystemName { get; private set; } = null;
        public SystemDescription SystemDescription { get; private set; } = null;
        public SystemCapabilities SystemCapabilities { get; private set; } = null;
        public ManagementAddress ManagementAddress { get; private set; } = null;
        public OrganizationSpecific OrganizationSpecific { get; private set; } = null;

        public DateTime Expires { get; private set; }
        public Interface Interface { get; private set; }

        public LLDPEntry(TLVCollection TlvCollection, Interface Interface)
        {
            foreach (TLV tlv in TlvCollection)
            {
                if (tlv.GetType() == typeof(EndOfLLDPDU))
                {
                    break;
                }

                if (tlv.GetType() == typeof(ChassisID))
                {
                    ChassisID = (ChassisID)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(PortID))
                {
                    PortID = (PortID)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(TimeToLive))
                {
                    TimeToLive = (TimeToLive)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(PortDescription))
                {
                    PortDescription = (PortDescription)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(SystemName))
                {
                    SystemName = (SystemName)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(SystemDescription))
                {
                    SystemDescription = (SystemDescription)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(SystemCapabilities))
                {
                    SystemCapabilities = (SystemCapabilities)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(ManagementAddress))
                {
                    ManagementAddress = (ManagementAddress)tlv;
                    continue;
                }

                if (tlv.GetType() == typeof(OrganizationSpecific))
                {
                    OrganizationSpecific = (OrganizationSpecific)tlv;
                    continue;
                }
            }

            Expires = DateTime.Now + CacheTimeout;
            this.Interface = Interface;
        }

        public bool HasExpired { get => DateTime.Now > Expires; }

        public int ExpiresIn { get => (int)(Expires - DateTime.Now).TotalSeconds; }
    }
}
