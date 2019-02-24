namespace Router.Controllers.Global
{
    class UpdateTables : Controller
    {
        public object ARP => new ARP.Table().Export();

        public object Export() => this;
    }
}
