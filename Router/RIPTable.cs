using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router
{
    class RIPTable
    {
        internal static RIPTable Instance { get; } = new RIPTable();

        private List<RIPEntry> Entries = new List<RIPEntry>();
        
        public bool Remove(Interface Interface, IPNetwork IPNetwork)
        {
            var index = Entries.FindIndex(Entry => Entry.Interface == Interface && Entry.IPNetwork == IPNetwork);
            if (index != -1)
            {
                Entries.RemoveAt(index);
            }

            return index != -1;
        }
    }
}
