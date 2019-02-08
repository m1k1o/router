﻿using Router.Protocols;
using System.Collections.Generic;
using System.Net;

namespace Router.RIP
{
    class RIPResponse
    {
        static public bool SplitHorizon { get; set; } = true;
        static public bool PoisonReverse { get; set; } = false;

        public Interface Interface { get; private set; }

        public RIPResponse(Interface Interface)
        {
            this.Interface = Interface;
        }

        public void Send()
        {
            var RIPEntries = RIPTable.Instance.BestEntries();
            Send(RIPEntries);
        }

        public void Send(List<RIPEntry> RIPEntries)
        {
            var RouteCollection = Export(RIPEntries);
            Protocols.RIP.Send(RIPCommandType.Response, RouteCollection, Interface);
        }

        public void Send(RIPRequest RIPRequest)
        {
            Protocols.RIP.Send(RIPRequest.SrcMac, RIPRequest.SrcIP, RIPRequest.SrcPort, RIPCommandType.Response, RIPRequest.RouteCollection, Interface);
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
                if (Interface.IsReachable(RIPEntry.NextHopIP))
                {
                    NextHop = RIPEntry.NextHopIP;
                }

                RouteCollection.Add(RIPEntry.IPNetwork, Interface.IPAddress, Metric);
            }

            return RouteCollection;
        }

        public static void SendTriggeredUpdate(Interface SourceInterface, List<RIPEntry> RIPEntries)
        {
            var Interfaces = RIPInterfaces.GetActiveInterfaces();
            foreach (var Interface in Interfaces)
            {
                if (Equals(Interface, SourceInterface))
                {
                    continue;
                }

                var RIPResponse = new RIPResponse(Interface);
                RIPResponse.Send(RIPEntries);
            }
        }

        public static void SendTriggeredUpdate(Interface SourceInterface, RIPEntry RIPEntry)
        {
            var ChangedRoutes = new List<RIPEntry>();
            ChangedRoutes.Add(RIPEntry);
            SendTriggeredUpdate(SourceInterface, RIPEntry);
        }

        static public void OnReceived(IPAddress SourceIP, RIPRouteCollection RouteCollection, Interface Interface)
        {
            var ChangedRIPEntries = new List<RIPEntry>();

            var RIPEntries = RIPTable.Instance.FindAll(Interface);
            foreach (var Route in RouteCollection)
            {
                bool RIPEntryChanged = false;
                uint Metric = Route.Metric + 1;

                IPAddress NextHopIP = SourceIP;
                if (Interface.IsReachable(SourceIP))
                {
                    NextHopIP = Route.NextHop;
                }

                var RIPEntry = RIPEntries.Find(Route.IPNetwork);
                if (RIPEntry != null)
                {
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

                    RIPEntry = RIPTable.Instance.Add(Interface, Route.IPNetwork, NextHopIP, Metric);
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
                SendTriggeredUpdate(Interface, ChangedRIPEntries);
            }
        }
    }
}
