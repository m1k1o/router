namespace Router.Controllers.Routing
{
    class Initialize : Controller
    {
        public object Table => new Table().Export();

        public object Export() => this;
    }
}
