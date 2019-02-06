using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    public class RTECollection : List<RTE>
    {
        public override string ToString()
        {
            var result = "RTECollection:\n\n";
            foreach (var item in this)
            {
                result += item.ToString() + "\n\n";
            }

            return result;
        }
    }
}
