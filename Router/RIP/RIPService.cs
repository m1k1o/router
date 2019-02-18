using PacketDotNet;
using Router.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Router.RIP
{
    class RIPService : InterfaceService
    {
        public string Name { get; } = "rip";

        public string Description { get; } = "RIP";

        public bool OnlyRunningInterface { get; } = true;

        public bool DefaultRunning { get; } = false;

        public bool Anonymous { get; } = false;

        public void OnStarted(Interface Interface)
        {
            var RIPEntry = new RIPEntry(Interface, Interface.IPNetwork, Interface.IPAddress, 1)
            {
                SyncWithRT = false,
                CanBeUpdated = false,
                TimersEnabled = false
            };

            RIPTable.Instance.Add(RIPEntry);
            RIPUpdates.SendTriggered(Interface, RIPEntry);
            RIPRequest.AskForUpdate(Interface);
            RIPUpdates.Add(Interface);
        }

        public void OnStopped(Interface Interface)
        {
            var RIPEntry = RIPTable.Instance.Find(Interface, Interface.IPNetwork);
            if (RIPEntry == null)
            {
                throw new Exception("RIPEntry not found while Stopping");
            }

            RIPEntry.PossibblyDown = true;

            RIPUpdates.SendTriggered(Interface, RIPEntry);
            RIPTable.Instance.Remove(Interface);
            RIPTable.Instance.SyncWithRT();
            RIPUpdates.Remove(Interface);
        }

        public void OnChanged(Interface Interface)
        {
            // Poison old
            var OldRIPEntry = RIPTable.Instance.FindAll(Interface).Where(Entry => Entry.Metric == 1).First();
            if (OldRIPEntry == null)
            {
                throw new Exception("RIPEntry not found while Stopping");
            }

            OldRIPEntry.PossibblyDown = true;

            // Remove all routes from that interface ?
            RIPTable.Instance.Remove(Interface);

            // Insert new
            var NewRIPEntry = new RIPEntry(Interface, Interface.IPNetwork, Interface.IPAddress, 1)
            {
                SyncWithRT = false,
                CanBeUpdated = false,
                TimersEnabled = false
            };
            RIPTable.Instance.Add(NewRIPEntry);

            RIPUpdates.SendTriggered(Interface, new List<RIPEntry> {
                OldRIPEntry,
                NewRIPEntry
            });
            RIPRequest.AskForUpdate(Interface);
            RIPTable.Instance.SyncWithRT();
        }

        public void OnPacketArrival(Handler Handler)
        {
            if (!Protocols.RIP.Validate(Handler))
            {
                return;
            }

            RIPPacket RIPPacket = Protocols.RIP.Parse(Handler.UdpPacket);

            Console.WriteLine("Got RIP.");
            IPv4Packet IPPacket = (IPv4Packet)Handler.EthernetPacket.Extract(typeof(IPv4Packet));

            if (RIPPacket.CommandType == RIPCommandType.Request)
            {
                UdpPacket UDPPacket = (UdpPacket)IPPacket.Extract(typeof(UdpPacket));
                RIPRequest.OnReceived(Handler.EthernetPacket.SourceHwAddress, IPPacket.SourceAddress, UDPPacket.SourcePort, RIPPacket.RouteCollection, Handler.Interface);
                return;
            }

            if (RIPPacket.CommandType == RIPCommandType.Response)
            {
                RIPResponse.OnReceived(IPPacket.SourceAddress, RIPPacket.RouteCollection, Handler.Interface);
                return;
            }
        }
    }
}
