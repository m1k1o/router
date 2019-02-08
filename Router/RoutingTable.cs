using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router
{
    class RoutingTable
    {
        internal static RoutingTable Instance { get; } = new RoutingTable();

        private List<RoutingEntry> Entries = new List<RoutingEntry>();

        static int LookupMaxTries = 128;

        public void Push(RoutingEntry RoutingEntry)
        {
            var Route = Entries.Find(Entry => Entry.IPNetwork == RoutingEntry.IPNetwork && Entry.ADistance == RoutingEntry.ADistance);
            if (Route != null)
            {
                throw new Exception("Route " + Route + " is already in Routing Table.");
            }

            Entries.Add(RoutingEntry);
        }

        public RoutingEntry Find(IPNetwork IPNetwork, ADistance ADistance)
        {
            return Entries.Find(Entry => Entry.IPNetwork == IPNetwork && Entry.ADistance == ADistance);
        }

        public RoutingEntry BestMatch(IPAddress IPAddress)
        {
            RoutingEntry BestMatch = null;

            var Entries = GetEntries();
            foreach (var Entry in Entries)
            {
                if (!Entry.IPNetwork.Contains(IPAddress))
                {
                    continue;
                }

                if (BestMatch == null || BestMatch.IPNetwork >= Entry.IPNetwork || BestMatch.ADistance < Entry.ADistance)
                {
                    BestMatch = Entry;
                }
            }

            return BestMatch;
        }

        public RoutingEntry Lookup(IPAddress TargetIP)
        {
            RoutingEntry ResultRoute = null;
            IPAddress NextHopIp = TargetIP;

            int MaxTries = LookupMaxTries;
            while (MaxTries-- > 0)
            {
                RoutingEntry Found = BestMatch(NextHopIp);
                if (Found == null)
                {
                    break;
                }

                if (Found.HasNextHopIP)
                {
                    NextHopIp = Found.NextHopIP;
                }

                ResultRoute = Found;

                if (Found.HasInterface || !Found.HasNextHopIP)
                {
                    break;
                }
            }

            if (ResultRoute == null)
            {
                return null;
            }

            ResultRoute = ResultRoute.Clone();
            ResultRoute.NextHopIP = NextHopIp;
            return ResultRoute;
        }

        public bool Remove(IPNetwork IPNetwork, ADistance ADistance)
        {
            var index = Entries.FindIndex(Entry => Entry.IPNetwork == IPNetwork && Entry.ADistance == ADistance);
            if (index != -1)
            {
                Entries.RemoveAt(index);
            }

            return index != -1;
        }

        public void PushDirectlyConnected(Interface Interface, IPNetwork IPNetwork)
        {
            Push(new RoutingEntry(IPNetwork, null, Interface, ADistance.DirectlyConnected));
        }

        public void RemoveDirectlyConnected(Interface Interface)
        {
            Entries.RemoveAll(Entry => Entry.Interface == Interface && Entry.ADistance == ADistance.DirectlyConnected);
        }

        public bool Exists(IPNetwork IPNetwork)
        {
            return Entries.Exists(Entry => Entry.IPNetwork == IPNetwork);
        }

        public bool Exists(IPAddress IPAddress)
        {
            return Entries.Exists(Entry => Entry.IPNetwork.Contains(IPAddress));
        }

        public List<RoutingEntry> GetEntries()
        {
            return Entries.ToList();
        }
    }
}
