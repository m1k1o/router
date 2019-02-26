namespace Router.Controllers.Interfaces
{
    class Initialize : Controller
    {
        public object Table => new Table().Export();

        public object Services => new Services().Export();

        public object Export() => this;
    }
}
