using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Router.RIP
{
    class RIPTable
    {
        public static RIPTable Instance { get; } = new RIPTable();

        private List<RIPEntry> Entries = new List<RIPEntry>();

        private RIPTable()
        {

        }

        public void Add(RIPEntry Entry)
        {
            // TODO: Don't allow duplicities
            Entries.Add(Entry);
        }

        public List<RIPEntry> FindAll(Interface Interface)
        {
            return Entries.FindAll(Entry => Equals(Entry.Interface, Interface) && !Entry.ToBeRemoved);
        }

        public List<RIPEntry> FindAll(IPNetwork IPNetwork)
        {
            return Entries.FindAll(Entry => Equals(Entry.IPNetwork, IPNetwork) && !Entry.ToBeRemoved);
        }

        public RIPEntry Find(Interface Interface, IPNetwork IPNetwork)
        {
            return Entries.Find(Entry => Equals(Entry.Interface, Interface) && Equals(Entry.IPNetwork, IPNetwork) && !Entry.ToBeRemoved);
        }

        public void Flush()
        {
            Entries = new List<RIPEntry>();
        }

        public List<RIPEntry> BestEntries()
        {
            return GetEntries().GroupBy(
                Entry => Entry.IPNetwork,
                Entry => Entry,
                (BaseIPNetwork, Routes) => Routes.BestRoute()).ToList();
        }

        public List<RIPEntry> GetEntries()
        {
            return Entries.FindAll(Entry => !Entry.ToBeRemoved);
        }

        public void Remove(Interface Interface)
        {
            Entries.RemoveAll(RIPEntry => {
                if(!Equals(RIPEntry.Interface, Interface))
                {
                    return false;
                }

                // Remove
                RoutingTable.Instance.Remove(RIPEntry.IPNetwork, ADistance.RIP);
                return true;
            });
        }

        public void GarbageCollector()
        {
            Entries.RemoveAll(RIPEntry => {
                if (!RIPEntry.SyncWithRT || !RIPEntry.ToBeRemoved)
                {
                    // Remove from RT
                    RoutingTable.Instance.Remove(RIPEntry.IPNetwork, ADistance.RIP);
                }

                return RIPEntry.ToBeRemoved;
            });
        }

        private int LastHashCode;

        public void SyncWithRT()
        {
            GarbageCollector();

            var Entries = BestEntries();

            var HashCode = RIPListExtensions.GetHashCode(Entries);
            if (Entries.Count == 0 || LastHashCode == HashCode)
            {
                return;
            }
            LastHashCode = HashCode;

            foreach (var Entry in Entries)
            {
                if (!Entry.SyncWithRT)
                {
                    continue;
                }

                var RoutingEntry = new RoutingEntry(Entry.IPNetwork, Entry.NextHopIP, Entry.Interface, ADistance.RIP);

                try
                {
                    RoutingTable.Instance.Push(RoutingEntry);
                }
                catch { }
            }
        }
    }

    static class RIPListExtensions
    {
        public static RIPEntry BestRoute(this IEnumerable<RIPEntry> Entries)
        {
            return Entries.OrderBy(Entry => Entry.Metric).First();
        }

        public static bool Exists(this List<RIPEntry> Entries, IPNetwork IPNetwork)
        {
            return Entries.Exists(Entry => Entry.IPNetwork == IPNetwork);
        }

        public static RIPEntry Find(this List<RIPEntry> Entries, IPNetwork IPNetwork)
        {
            return Entries.Find(Entry => Entry.IPNetwork == IPNetwork);
        }

        public static int GetHashCode(List<RIPEntry> Entries)
        {
            unchecked
            {
                int hash = 19;
                foreach (var Entry in Entries)
                {
                    hash = hash * 31 + Entry.GetHashCode();
                }
                return hash;
            }
        }
    }
}
