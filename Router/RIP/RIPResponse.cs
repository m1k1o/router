using Router.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    class RIPResponse
    {
        public static bool SplitHorizon = true;
        public static bool PoisonReverse = false;

        public Interface Interface { get; private set; }

        public RIPResponse(Interface Interface)
        {
            this.Interface = Interface;
        }

        public static void OnReceived(IPAddress SourceIP, RIPRouteCollection RouteCollection, Interface Interface)
        {
            var ChangedRIPEntries = new List<RIPEntry>();

            var RIPEntries = RIPTable.Instance.Find(Interface);
            foreach (var Route in RouteCollection)
            {
                bool RIPEntryChanged = false;

                IPAddress NextHopIP = SourceIP;
                if (Interface.IPNetwork.Contains(Route.NextHop) && !Interface.IPNetwork.Equals(Route.NextHop))
                {
                    NextHopIP = Route.NextHop;
                }

                var RIPEntry = RIPEntries.Find(Route.IPNetwork);
                if (RIPEntry != null)
                {
                    if (Route.Metric == 16)
                    {
                        RIPEntry.InHold = true;
                    }
                    else
                    {
                        if (!RIPEntry.InHold)
                        {
                            RIPEntryChanged = RIPEntry.Update(NextHopIP, Route.Metric);
                        }
                    }
                }
                else
                {
                    // Don't add poisoned routes
                    if (Route.Metric == 16)
                    {
                        continue;
                    }

                    RIPEntry = RIPTable.Instance.Add(Interface, Route.IPNetwork, NextHopIP, Route.Metric);
                    RIPEntryChanged = true;
                }

                if (RIPEntryChanged)
                {
                    ChangedRIPEntries.Add(RIPEntry);
                }
            }

            if (ChangedRIPEntries.Count > 0)
            {
                // Send triggered update
                throw new NotImplementedException();
            }
        }

        public void SendUpdate()
        {
            var RIPEntries = RIPTable.Instance.BestEntries();
            SendTriggeredUpdate(RIPEntries);
        }

        public void SendTriggeredUpdate(List<RIPEntry> RIPEntries)
        {
            var RouteCollection = Export(RIPEntries);
            Protocols.RIP.Send(RIPCommandType.Response, RouteCollection, Interface);
        }
        /*
        public void SendResponse(RIPRequest RIPRequest)
        {
            throw new NotImplementedException();
        }
        */
        public RIPRouteCollection Export(List<RIPEntry> RIPEntries)
        {
            var RouteCollection = new RIPRouteCollection();

            foreach (var RIPEntry in RIPEntries)
            {
                uint Metric;

                // Split Hotizon \w Poison Reverse
                if (SplitHorizon && RIPEntry.Interface == Interface)
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
                    if (RIPEntry.InHold)
                    {
                        Metric = 16;
                    }
                    else
                    {
                        Metric = RIPEntry.Metric;
                    }
                }

                RouteCollection.Add(RIPEntry.IPNetwork, Interface.IPAddress, Metric);
            }

            return RouteCollection;
        }
    }
}
