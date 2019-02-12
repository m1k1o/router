using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.LLDP
{
    class LLDPService : InterfaceService
    {
        public string Name { get; } = "lldp";

        public string Description { get; } = "LLDP";

        public bool OnlyRunningInterface { get; } = true;

        public bool DefaultRunning { get; } = false;

        public void OnStarted(Interface Interface)
        {
            LLDP.LLDPAdvertisements.Add(Interface);
        }

        public void OnStopped(Interface Interface)
        {
            LLDP.LLDPAdvertisements.Remove(Interface);
        }

        public void OnChanged(Interface Interface) { }

        public void OnPacketArrival(Handler Handler)
        {
            if (!Handler.CheckType(typeof(LLDPPacket)))
            {
                return;
            }

            LLDPResponse.OnReceived(Handler.Interface, (LLDPPacket)Handler.PacketPayload);
        }
    }
}
