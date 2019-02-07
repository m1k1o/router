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


        public static void Import(RIPRouteCollection RouteCollection, Interface Interface)
        {
            throw new NotFiniteNumberException();
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

                RouteCollection.Add(Route.IPNetwork.NetworkAddress, Route.IPNetwork.SubnetMask, Interface.IPAddress, Metric);
            }

            return RouteCollection;
        }
    }
}
