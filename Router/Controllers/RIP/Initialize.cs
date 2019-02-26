namespace Router.Controllers.RIP
{
    class Initialize : Controller
    {
        public object Table => new Table().Export();
        public object Timers => new Timers().Export();

        public object Export() => this;
    }
}
