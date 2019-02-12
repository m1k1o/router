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
            if (!Handler.CheckType(typeof(IPv4Packet)))
            {
                return;
            }

            IPv4Packet IPv4Packet = (IPv4Packet)Handler.PacketPayload;

            Console.WriteLine("Got IPV4.");
            if (
                // Is from my MAC
                Equals(Handler.EthernetPacket.SourceHwAddress, Handler.Interface.PhysicalAddress) ||

                // Is not to my MAC
                !Equals(Handler.EthernetPacket.DestinationHwAddress, Handler.Interface.PhysicalAddress) ||

                // Is to my IP
                Equals(IPv4Packet.DestinationAddress, Handler.Interface.IPAddress) ||

                // Is to my Device IP
                (Handler.Interface.DeviceIP != null && Equals(IPv4Packet.DestinationAddress, Handler.Interface.DeviceIP))
            )
            {
                return;
            }

            Routing.AddToQueue(Handler);
        }
    }
}
