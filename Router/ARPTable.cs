using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Router
{
    class ARPTable
    {
        internal static ARPTable Instance { get; } = new ARPTable();

        public int TTL = 128;

        private List<ARPEntry> Entries = new List<ARPEntry>();

        public void Push(IPAddress IPAddress, PhysicalAddress PhysicalAddress)
        {
            long timeStamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            foreach (var Entry in Entries)
            {
                if (Equals(Entry.IPAddress, IPAddress))
                {
                    Entry.PhysicalAddress = PhysicalAddress;
                    Entry.TTL = timeStamp + TTL;
                    return;
                }
            }

            Entries.Add(new ARPEntry(IPAddress, PhysicalAddress, timeStamp + TTL));
        }

        public PhysicalAddress Find(IPAddress IPAddress)
        {
            long timeStamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            foreach (var Entry in Entries)
            {
                if (Equals(Entry.IPAddress, IPAddress))
                {
                    if (Entry.TTL < timeStamp)
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
