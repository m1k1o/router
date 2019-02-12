using Router.Protocols;
using System.Collections.Generic;
using System.Net;

namespace Router.RIP
{
    class RIPResponse
    {
        public static bool SplitHorizon { get; set; } = true;
        public static bool PoisonReverse { get; set; } = false;

        public Interface Interface { get; private set; }

        public RIPResponse(Interface Interface)
        {
            this.Interface = Interface;
        }

        public void Send()
        {
            var RouteCollection = Export();

            if (RouteCollection.Count == 0)
            {
                return;
            }

            Protocols.RIP.Send(RIPCommandType.Response, RouteCollection, Interface);
        }

        public void Send(List<RIPEntry> RIPEntries)
        {
            var RouteCollection = Export(RIPEntries);

            if (RouteCollection.Count == 0)
            {
                return;
            }

            Protocols.RIP.Send(RIPCommandType.Response, RouteCollection, Interface);
        }

        public void Send(RIPRequest RIPRequest)
        {
            RIPRouteCollection RouteCollection;
            if (RIPRequest.AskingForUpdate)
            {
                RouteCollection = Export();
            }
            else
            {
                RIPRequest.Export(Interface);
                RouteCollection = RIPRequest.RouteCollection;
            }

            if (RouteCollection.Count == 0)
            {
                return;
            }

            Protocols.RIP.Send(RIPRequest.SrcMac, RIPRequest.SrcIP, RIPRequest.SrcPort, RIPCommandType.Response, RouteCollection, Interface);
        }

        private RIPRouteCollection Export()
        {
            var RIPEntries = RIPTable.Instance.BestEntries();
            return Export(RIPEntries);
        }

        private RIPRouteCollection Export(List<RIPEntry> RIPEntries)
        {
            var RouteCollection = new RIPRouteCollection();

            foreach (var RIPEntry in RIPEntries)
            {
                uint Metric;

                // Split Hotizon \w Poison Reverse
                if (SplitHorizon && Equals(RIPEntry.Interface, Interface))
                {
                    if (PoisonReverse)
                    {
                        Metric = 16;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (RIPEntry.PossibblyDown)
                    {
                        Metric = 16;
                    }
                    else
                    {
                        Metric = RIPEntry.Metric;
                    }
                }

                IPAddress NextHop = Interface.IPAddress;
                if (RIPEntry.NextHopIP != null && Interface.IsReachable(RIPEntry.NextHopIP))
                {
                    NextHop = RIPEntry.NextHopIP;
                }

                RouteCollection.Add(RIPEntry.IPNetwork, Interface.IPAddress, Metric);
            }

            return RouteCollection;
        }

        public static void OnReceived(IPAddress SourceIP, RIPRouteCollection RouteCollection, Interface Interface)
        {
            var ChangedRIPEntries = new List<RIPEntry>();

            var RIPEntries = RIPTable.Instance.FindAll(Interface);
            foreach (var Route in RouteCollection)
            {
                if (Route.Metric > 16 || Route.AddressFamilyIdentifier != 2)
                {
                    continue;
                }

                bool RIPEntryChanged = false;
                uint Metric = Route.Metric == 16 ? 16 : Route.Metric + 1;

                IPAddress NextHopIP = SourceIP;
                if (Interface.IsReachable(Route.NextHop))
                {
                    NextHopIP = Route.NextHop;
                }

                var RIPEntry = RIPEntries.Find(Route.IPNetwork);
                if (RIPEntry != null)
                {
                    if (!RIPEntry.CanBeUpdated)
                    {
                        continue;
                    }

                    if (Metric == 16)
                    {
                        RIPEntry.PossibblyDown = true;
                        RIPEntry.SyncWithRT = false;
                        RIPEntryChanged = true;
                    }
                    else
                    {
                        if (!RIPEntry.InHold)
                        {
                            RIPEntryChanged = RIPEntry.Update(NextHopIP, Metric);
                        }
                    }
                }
                else
                {
                    // Don't add poisoned routes
                    if (Metric == 16)
                    {
                        continue;
                    }

                    RIPEntry = new RIPEntry(Interface, Route.IPNetwork, NextHopIP, Metric);
                    RIPTable.Instance.Add(RIPEntry);
                    RIPEntryChanged = true;
                }

                if (RIPEntryChanged)
                {
                    ChangedRIPEntries.Add(RIPEntry);
                }
            }

            if (ChangedRIPEntries.Count > 0)
            {
                RIPTable.Instance.SyncWithRT();
                RIPUpdates.SendTriggered(Interface, ChangedRIPEntries);
            }
        }
    }
}
