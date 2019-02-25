namespace Router.Controllers.Global
{
    class UpdateTables : Controller
    {
        //public object Interfaces => new Interfaces.Table().Export();
        public object ARP => new ARP.Table().Export();
        public object Routing => new Routing.Table().Export();
        public object RIP => new RIP.Table().Export();
        public object LLDP => new LLDP.Table().Export();
        public object DHCP => new DHCP.Table().Export();

        public object Sniffing => new Sniffing.Pop().Export();

        public object Export() => this;
    }
}
