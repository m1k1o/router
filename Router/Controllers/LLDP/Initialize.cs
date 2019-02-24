namespace Router.Controllers.LLDP
{
    class Initialize : Controller
    {
        public object Table => new Table().Export();
        public object Settings => new Settings().Export();

        public object Export() => this;
    }
}
