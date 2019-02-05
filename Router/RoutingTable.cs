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

        public void Push(Interface Interface, IPNetwork IPNetwork)
        {
            var index = Entries.FindIndex(Entry => Entry.Interface == Interface && Entry.ADistance == ADistance.DirectlyConnected);
            if (index != -1)
            {
                Entries[index].IPNetwork = IPNetwork;
                Entries[index].NextHopIP = null;
                return;
            }

            Entries.Add(new RoutingEntry(IPNetwork, null, Interface, ADistance.DirectlyConnected));
        }

        public void Push(RoutingEntry RoutingEntry)
        {
            var index = Entries.FindIndex(Entry => Entry.IPNetwork == RoutingEntry.IPNetwork && Entry.ADistance == RoutingEntry.ADistance);
            if (index != -1)
            {
                Entries[index] = RoutingEntry;
                return;
            }

            Entries.Add(RoutingEntry);
        }

        public RoutingEntry Find(IPNetwork IPNetwork, ADistance ADistance)
        {
            return Entries.Find(Entry => Entry.IPNetwork == IPNetwork && Entry.ADistance == ADistance);
        }

        public RoutingEntry Lookup(IPAddress IPAddress)
        {
            RoutingEntry BestMatch = null;
            foreach (var Entry in Entries)
            {
                if (!Entry.IPNetwork.Contains(IPAddress))
                {
                    continue;
                }

                if(BestMatch == null || BestMatch.IPNetwork >= Entry.IPNetwork || BestMatch.ADistance < Entry.ADistance)
                {
                    if(BestMatch.NextHopIP == null)
                    {
                        Entry.NextHopIP = BestMatch.NextHopIP;
                    }

                    BestMatch = Entry;
                }
            }

            return BestMatch;
        }

        public bool Remove(IPNetwork IPNetwork, ADistance ADistance)
        {
            var index = Entries.FindIndex(Entry => Entry.IPNetwork == IPNetwork && Entry.ADistance == ADistance);
            if(index != -1)
            {
                Entries.RemoveAt(index);
                return true;
            }

            return false;
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
            return Entries;
        }
    }
}
