using Router.Helpers;
using System.Collections.Generic;

namespace Router.Sniffing
{
    static class SniffingList
    {
        public static int MaxEntries { get; set; } = 50;
        private static int TotalEntries = 0;

        private static List<old_JSON> Entries = new List<old_JSON>();

        public static Interface Interface { get; set; }

        public static old_JSONArray Pop()
        {
            List<old_JSON> OProcessingEntries;

            lock (Entries)
            {
                OProcessingEntries = Entries;
                Entries = new List<old_JSON>();
                TotalEntries = 0;
            }

            return new old_JSONArray(OProcessingEntries);
        }

        public static void Push(old_JSON Input)
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
