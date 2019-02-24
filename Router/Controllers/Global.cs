namespace Router.Controllers
{
    static class Global
    {
        /*
        public static old_JSON Initialize(string Data = null)
        {
            var obj = new old_JSONObject();
            obj.Add("interfaces", Interfaces.Initialize());
            obj.Add("arp", ARP.Initialize());
            obj.Add("routing", Routing.Initialize());
            obj.Add("rip", RIP.Initialize());
            obj.Add("lldp", LLDP.Initialize());
            obj.Add("sniffing", Sniffing.Initialize());
            obj.Add("dhcp", DHCP.Initialize());
            return obj;
        }

        public static old_JSON UpdateTables(string Data = null)
        {
            var obj = new old_JSONObject();
            //obj.Push("interfaces", Interfaces.Table());
            obj.Add("arp", ARP.Table());
            obj.Add("routing", Routing.Table());
            obj.Add("rip", RIP.Table());
            obj.Add("lldp", LLDP.Table());
            obj.Add("sniffing", Sniffing.Pop());
            obj.Add("dhcp", DHCP.Table());
            return obj;
        }
        */

        public static object Initialize(string Data = null)
        {
            return new
            {
                interfaces = Interfaces.Initialize(),
                arp = ARP.Initialize(),
                //routing = Routing.Initialize(),
                //rip = RIP.Initialize(),
                //lldp = LLDP.Initialize(),
                //sniffing = Sniffing.Initialize(),
                //dhcp = DHCP.Initialize()
            };
        }

        public static object UpdateTables(string Data = null)
        {
            return new
            {
                //interfaces = Interfaces.Table(),
                arp = ARP.Table(),
                //routing = Routing.Table(),
                //rip = RIP.Table(),
                //lldp = LLDP.Table(),
                //sniffing = Sniffing.Pop(),
                //dhcp = DHCP.Table(),
            };
        }
    }
}
