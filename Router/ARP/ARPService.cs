﻿using PacketDotNet;
using System;

namespace Router.ARP
{
    class ARPService : InterfaceService
    {
        public string Name { get; } = "arp";

        public string Description { get; } = "ARP";

        public bool OnlyRunningInterface { get; } = true;

        public bool DefaultRunning { get; } = true;

        public bool Anonymous { get; } = true;

        public void OnStarted(Interface Interface) { }

        public void OnStopped(Interface Interface) { }

        public void OnChanged(Interface Interface) { }

        public void OnPacketArrival(Handler Handler)
        {
            if (!Handler.CheckEtherType(EthernetPacketType.Arp))
            {
                return;
            }

            var ARPPacket = (ARPPacket)Handler.InternetLinkLayerPacket.PayloadPacket;

            Console.WriteLine("Got ARP.");
            ARPMiddleware.OnReceived(Handler.EthernetPacket.DestinationHwAddress, ARPPacket, Handler.Interface);
        }
    }
}
