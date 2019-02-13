using Router.Helpers;
using System.Collections.Generic;

namespace Router.Sniffing
{
    class SniffingList
    {
        public static int MaxEntries { get; set; } = 50;
        private static int TotalEntries = 0;

        private static List<JSON> Entries = new List<JSON>();
        
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
            if (TotalEntries > MaxEntries)
            {
                return;
            }

            TotalEntries++;

            Entries.Add(Input);
        }
    }
}
