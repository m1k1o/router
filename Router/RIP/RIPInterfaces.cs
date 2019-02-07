using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    class RIPInterfaces : List<Interface>
    {
        internal static RIPInterfaces Instance { get; } = new RIPInterfaces();

        private RIPInterfaces()
        {

        }

        public void Add(int ID)
        {
            Add(Interfaces.Instance.GetInterfaceById(ID));
        }

        public void Add(string Name)
        {
            Add(Interfaces.Instance.GetInterfaceByName(Name));
        }
    }
}
