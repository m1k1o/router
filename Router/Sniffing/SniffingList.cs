using Router.Helpers;
using System.Collections.Generic;

namespace Router.Sniffing
{
    static class SniffingList
    {
        public static int MaxEntries { get; set; } = 50;
        private static int TotalEntries = 0;

        private static List<JSON> Entries = new List<JSON>();

        public static Interface Interface { get; set; }

        public static JSONArray Pop()
        {
            List<JSON> OProcessingEntries;

            lock (Entries)
            {
                OProcessingEntries = Entries;
                Entries = new List<JSON>();
                TotalEntries = 0;
            }

            return new JSONArray(OProcessingEntries);
        }

        public static void Push(JSON Input)
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
