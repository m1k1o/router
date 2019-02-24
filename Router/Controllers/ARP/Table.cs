using Router.ARP;
using System.Linq;

namespace Router.Controllers.ARP
{
    class Table : Controller
    {
        public bool Flush
        {
            set
            {
                if(value) ARPTable.Instance.Flush();
            }
        }

        public object Export() => ARPTable.Instance.GetEntries().Where(Row => !Row.HasExpired);
    }
}
