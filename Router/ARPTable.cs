using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPTable
    {
        public static ARPTable Instance { get; } = new ARPTable();

        private List<ARPEntry> Entries = new List<ARPEntry>();

        private ARPTable()
        {

        }

        public void Push(IPAddress IPAddress, PhysicalAddress PhysicalAddress)
        {
            var FoundEntry = Entries.Find(Entry => Equals(Entry.IPAddress, IPAddress));
            if (FoundEntry != null)
            {
                FoundEntry.Update(PhysicalAddress);
                return;
            }

            Entries.Add(new ARPEntry(IPAddress, PhysicalAddress));
        }

        public PhysicalAddress Find(IPAddress IPAddress)
        {
            var FoundEntry = Entries.Find(Entry => Equals(Entry.IPAddress, IPAddress) && !Entry.HasExpired);
            if (FoundEntry != null)
            {
                return FoundEntry.PhysicalAddress;
            }

            return null;
        }

        public void Flush()
        {
            Entries = new List<ARPEntry>();
        }

        public List<ARPEntry> GetEntries()
        {
            return Entries.ToList();
        }

        public void GarbageCollector()
        {
            Entries.RemoveAll(Entry => Entry.HasExpired);
        }
    }
}
