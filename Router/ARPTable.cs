using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPTable
    {
        internal static ARPTable Instance { get; } = new ARPTable();

        private List<ARPEntry> Entries = new List<ARPEntry>();

        public void Push(IPAddress IPAddress, PhysicalAddress PhysicalAddress)
        {
            foreach (var Entry in Entries)
            {
                if (Equals(Entry.IPAddress, IPAddress))
                {
                    Entry.Update(PhysicalAddress);
                    return;
                }
            }

            Entries.Add(new ARPEntry(IPAddress, PhysicalAddress));
        }

        public PhysicalAddress Find(IPAddress IPAddress)
        {
            foreach (var Entry in Entries)
            {
                if (Equals(Entry.IPAddress, IPAddress))
                {
                    if (Entry.HasExpired)
                    {
                        // Remove entry.
                        return null;
                    }

                    return Entry.PhysicalAddress;
                }
            }

            return null;
        }

        public void Flush()
        {
            Entries = new List<ARPEntry>();
        }

        public List<ARPEntry> GetEntries()
        {
            return Entries;
        }
    }
}
