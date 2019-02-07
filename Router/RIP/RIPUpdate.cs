using Router.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    class RIPUpdate
    {
        public static bool SplitHorizon = true;
        public static bool PoisonReverse = false;


        public static void Import(IPAddress SourceIP, RIPRouteCollection RouteCollection, Interface Interface)
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
                        RIPEntryChanged = RIPEntry.Update(NextHopIP, Route.Metric);
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

        public static RIPRouteCollection Export(Interface Interface)
        {
            var RouteCollection = new RIPRouteCollection();
            var Routes = RIPTable.Instance.BestEntries();

            foreach (var Route in Routes)
            {
                uint Metric;

                // Split Hotizon \w Poison Reverse
                if (SplitHorizon && Route.Interface == Interface)
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
                    Metric = Route.Metric;
                }

                RouteCollection.Add(Route.IPNetwork, Interface.IPAddress, Metric);
            }

            return RouteCollection;
        }
    }
}
