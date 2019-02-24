namespace Router.Controllers.Global
{
    class Initialize : Controller
    {
        public object ARP => new ARP.Initialize();

        public object Export() => this;
    }
}
