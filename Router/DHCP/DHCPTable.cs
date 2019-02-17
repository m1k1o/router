using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Router.DHCP
{
    class DHCPTable
    {
        public static DHCPTable Instance { get; } = new DHCPTable();

        private List<DHCPLease> Entries = new List<DHCPLease>();

        private DHCPTable() { }

        public void Push(DHCPLease DHCPLease)
        {
            Entries.Add(DHCPLease);
        }

        public DHCPLease Find(PhysicalAddress PhysicalAddress, Interface Interface)
        {
            return Entries.Find(Entry => Equals(Entry.PhysicalAddress, PhysicalAddress) && Equals(Entry.Interface, Interface));
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
