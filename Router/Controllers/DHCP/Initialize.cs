namespace Router.Controllers.DHCP
{
    class Initialize : Controller
    {
        public object Table => new Table().Export();
        public object Timers => new Timers().Export();
        public object Pools => new Pools().Export();

        public object Export() => this;
    }
}
