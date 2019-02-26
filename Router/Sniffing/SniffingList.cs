using Router.Packets;
using System.Collections.Generic;

namespace Router.Sniffing
{
    static class SniffingList
    {
        public static int MaxEntries { get; set; } = 50;
        private static int TotalEntries = 0;

        private static List<Ethernet> Entries = new List<Ethernet>();

        public static Interface Interface { get; set; }

        public static List<Ethernet> Pop()
        {
            List<Ethernet> OProcessingEntries;

            lock (Entries)
            {
                OProcessingEntries = Entries;
                Entries = new List<Ethernet>();
                TotalEntries = 0;
            }

            return OProcessingEntries;
        }

        public static void Push(Ethernet Input)
        {
            if (TotalEntries > MaxEntries || Input == null)
            {
                return;
            }

            lock (Entries)
            {
                TotalEntries++;
                Entries.Add(Input);
            }
        }
    }
}
