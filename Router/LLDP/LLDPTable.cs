using PacketDotNet;
using System.Collections.Generic;
using System.Linq;

namespace Router.LLDP
{
    static class LLDPTable
    {
        private static List<LLDPEntry> Entries = new List<LLDPEntry>();

        public static void Push(TLVCollection TLVCollection, Interface Interface)
        {
            LLDPEntry NewEntry = new LLDPEntry(TLVCollection, Interface);
            Entries.RemoveAll(Entry =>
                Entry.ChassisID.ToString() == NewEntry.ChassisID.ToString() &&
                Entry.PortID.ToString() == NewEntry.PortID.ToString()
            );

            Entries.Add(NewEntry);
        }

        public static List<LLDPEntry> GetEntries()
        {
            return Entries.ToList();
        }

        public static void Flush()
        {
            Entries = new List<LLDPEntry>();
        }
    }
}
