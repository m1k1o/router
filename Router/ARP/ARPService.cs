﻿using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.ARP
{
    class ARPService : InterfaceService
    {
        public string Name { get; } = "arp";

        public string Description { get; } = "ARP";

        public bool OnlyRunningInterface { get; } = false;

        public bool DefaultRunning { get; } = true;

        public void OnStarted(Interface Interface) { }

        public void OnStopped(Interface Interface) { }

        public void OnChanged(Interface Interface) { }

        public void OnPacketArrival(Handler Handler)
        {
            if (!Handler.CheckType(typeof(ARPPacket)))
            {
                return;
            }

            Console.WriteLine("Got ARP.");
            ARPMiddleware.OnReceived(Handler.EthernetPacket.DestinationHwAddress, (ARPPacket)Handler.PacketPayload, Handler.Interface);
        }
    }
}