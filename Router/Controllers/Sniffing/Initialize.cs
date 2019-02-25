namespace Router.Controllers.Sniffing
{
    class Initialize : Controller
    {
        public object Data => new Pop().Export();

        public object Interface => new Interface().ID;

        public object Export() => this;
    }
}
