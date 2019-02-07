using System.Collections.Generic;

namespace Router.Protocols
{
    public class RIPRouteCollection : List<RIPRoute>
    {
        public override string ToString()
        {
            var result = "RIP Route Collection:\n\n";
            foreach (var item in this)
            {
                result += item.ToString() + "\n\n";
            }

            return result;
        }
    }
}
