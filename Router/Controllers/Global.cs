using Router.Helpers;

namespace Router.Controllers
{
    static class Global
    {
        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("interfaces", Interfaces.Initialize());
            obj.Push("arp", ARP.Initialize());
            obj.Push("routing", Routing.Initialize());
            obj.Push("rip", RIP.Initialize());
            return obj;
        }

        public static JSON UpdateTables(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("interfaces", Interfaces.Table());
            obj.Push("arp", ARP.Table());
            obj.Push("routing", Routing.Table());
            obj.Push("rip", RIP.Table());
            return obj;
        }
    }
}
