using System.Collections.Generic;

namespace Router.Sniffing
{
    static class SniffingList
    {
        /*
        public static int MaxEntries { get; set; } = 50;
        private static int TotalEntries = 0;

        private static List<object> Entries = new List<object>();

        public static Interface Interface { get; set; }

        public static List<object> Pop()
        {
            List<object> OProcessingEntries;

            lock (Entries)
            {
                OProcessingEntries = Entries;
                Entries = new List<object>();
                TotalEntries = 0;
            }

            return OProcessingEntries;
        }

        public static void Push(object Input)
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
        */
    }
}
