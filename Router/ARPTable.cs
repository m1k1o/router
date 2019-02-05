using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPTable
    {
        internal static ARPTable Instance { get; } = new ARPTable();

        public TimeSpan CacheTimeout = TimeSpan.FromSeconds(128);

        private List<ARPEntry> Entries = new List<ARPEntry>();

        public void Push(IPAddress IPAddress, PhysicalAddress PhysicalAddress)
        {
            foreach (var Entry in Entries)
            {
                if (Equals(Entry.IPAddress, IPAddress))
                {
                    Entry.PhysicalAddress = PhysicalAddress;
                    Entry.Expires = DateTime.Now + CacheTimeout;
                    return;
                }
            }

            Entries.Add(new ARPEntry(IPAddress, PhysicalAddress, DateTime.Now + CacheTimeout));
        }

        public PhysicalAddress Find(IPAddress IPAddress)
        {
            foreach (var Entry in Entries)
            {
                if (Equals(Entry.IPAddress, IPAddress))
                {
                    if (DateTime.Now > Entry.Expires)
                    {
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
