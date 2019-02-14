using PacketDotNet;
using System;

namespace Router
{
    class RoutingService : InterfaceService
    {
        public string Name { get; } = "routing";

        public string Description { get; } = "Routing";

        public bool OnlyRunningInterface { get; } = true;

        public bool DefaultRunning { get; } = true;

        public bool Anonymous { get; } = true;

        private int TotalRunning = 0;

        public void OnStarted(Interface Interface)
        {
            if (TotalRunning == 0)
            {
                Routing.Start();
            }

            TotalRunning++;
        }

        public void OnStopped(Interface Interface)
        {
            TotalRunning--;

            if (TotalRunning == 0)
            {
                Routing.Stop();
            }
        }

        public void OnChanged(Interface Interface) { }

        public void OnPacketArrival(Handler Handler)
        {
            if (!Handler.CheckEtherType(EthernetPacketType.IpV4))
            {
                return;
            }

            Console.WriteLine("Got IPV4.");
            if (
                // Is not to my MAC
                !Equals(Handler.EthernetPacket.DestinationHwAddress, Handler.Interface.PhysicalAddress) ||

                // Is from my IP
                Equals(Handler.IPv4Packet.SourceAddress, Handler.Interface.IPAddress) ||

                // Is to my IP
                Equals(Handler.IPv4Packet.DestinationAddress, Handler.Interface.IPAddress) ||

                // Is to my Device IP
                (Handler.Interface.DeviceIP != null && Equals(Handler.IPv4Packet.DestinationAddress, Handler.Interface.DeviceIP))
            )
            {
                return;
            }

            Routing.AddToQueue(Handler);
        }
    }
}
