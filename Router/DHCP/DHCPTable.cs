using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public bool Exists(IPAddress IPAddress)
        {
            return Entries.Exists(Entry => Equals(Entry.IPAddress, IPAddress));
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

        public DHCPLease AddStatic(PhysicalAddress PhysicalAddress, Interface Interface, IPAddress IPAddress)
        {
            var Lease = new DHCPLease(PhysicalAddress, Interface, IPAddress)
            {
                IsDynamic = false
            };
            Push(Lease);
            return Lease;
        }

        public void RemoveStatic(PhysicalAddress PhysicalAddress, Interface Interface)
        {
            Entries.RemoveAll(Entry => Equals(Entry.PhysicalAddress, PhysicalAddress) && Equals(Entry.Interface, Interface));
        }

        public void Remove(DHCPLease DHCPLease)
        {
            if (DHCPLease.IsDynamic)
            {
                // Free from Pool
                if (!DHCPPool.Interfaces.ContainsKey(DHCPLease.Interface))
                {
                    return;
                }

                var Pool = DHCPPool.Interfaces[DHCPLease.Interface];
                Pool.Free(DHCPLease.IPAddress);
            }

            Entries.Remove(DHCPLease);
        }
    }
}
