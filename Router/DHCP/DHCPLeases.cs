using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Router.DHCP
{
    class DHCPLeases
    {
        public static DHCPLeases Instance { get; } = new DHCPLeases();

        private List<DHCPLease> Entries = new List<DHCPLease>();

        private DHCPLeases() { }

        public void Push(DHCPLease DHCPLease)
        {
            Entries.Add(DHCPLease);
        }

        public DHCPLease Find(PhysicalAddress PhysicalAddress)
        {
            return Entries.Find(Entry => Equals(Entry.PhysicalAddress, PhysicalAddress));
        }

        public void Flush()
        {
            Entries = new List<DHCPLease>();
        }

        public List<DHCPLease> GetEntries()
        {
            return Entries.ToList();
        }

        public void GarbageCollector()
        {
            Entries.RemoveAll(Entry => Entry.ToBeRemoved);
        }
    }
}
