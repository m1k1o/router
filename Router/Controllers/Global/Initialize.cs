namespace Router.Controllers.Global
{
    class Initialize : Controller
    {
        public object Interfaces => new Interfaces.Initialize().Export();
        public object ARP => new ARP.Initialize().Export();

        public object Export() => this;
    }
}
