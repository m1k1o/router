namespace Router.Controllers.Global
{
    class UpdateTables : Controller
    {
        //public object Interfaces => new Interfaces.Table().Export();
        public object ARP => new ARP.Table().Export();
        public object Routing => new Routing.Table().Export();

        public object Export() => this;
    }
}
