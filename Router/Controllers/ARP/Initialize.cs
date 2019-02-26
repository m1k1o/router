namespace Router.Controllers.ARP
{
    class Initialize : Controller
    {
        public object Table => new Table().Export();

        public object Timers => new Timers();

        public object Proxy => new Proxy();

        public object Export() => this;
    }
}
