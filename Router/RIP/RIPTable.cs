using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    class RIPTable
    {
        public static RIPTable Instance { get; } = new RIPTable();

        private List<RIPEntry> Entries = new List<RIPEntry>();

        public void Add(RIPEntry Entry)
        {
            // TODO: Don't allow duplicities
            Entries.Add(Entry);
        }

        public RIPEntry Add(Interface Interface, IPNetwork IPNetwork, IPAddress NextHopIP, uint Metric)
        {
            var Entry = new RIPEntry(Interface, IPNetwork, NextHopIP, Metric);
            Add(Entry);
            return Entry;
        }

        public List<RIPEntry> FindAll(Interface Interface)
        {
            return Entries.FindAll(Entry => Entry.Interface == Interface && !Entry.ToBeRemoved);
        }

        public List<RIPEntry> FindAll(IPNetwork IPNetwork)
        {
            return Entries.FindAll(Entry => Entry.IPNetwork == IPNetwork && !Entry.ToBeRemoved);
        }

        public RIPEntry Find(Interface Interface, IPNetwork IPNetwork)
        {
            return Entries.Find(Entry => Entry.Interface == Interface && Entry.IPNetwork == IPNetwork && !Entry.ToBeRemoved);
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
            Entries.RemoveAll(RIPEntry => RIPEntry.Interface == Interface);
        }

        public void GarbageCollector()
        {
            Entries.RemoveAll(Entry => Entry.ToBeRemoved);
        }

        public void SyncWithRT()
        {
            // TODO: Sync with RT.
            throw new NotImplementedException();

            // Do not import routes with SyncWithRT == false;

            var Entries = BestEntries();
            foreach(var Entry in Entries)
            {

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
    }
}
