namespace Router.Controllers.Global
{
    class Initialize : Controller
    {
        public object Interfaces => new Interfaces.Initialize().Export();
        public object TestCases => new Analyzer.GetAllTestCases().Export();

        public object ARP => new ARP.Initialize().Export();
        public object Routing => new Routing.Initialize().Export();
        public object RIP => new RIP.Initialize().Export();
        public object LLDP => new LLDP.Initialize().Export();
        public object DHCP => new DHCP.Initialize().Export();

        public object Export() => this;
    }
}
