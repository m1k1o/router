namespace Router.Controllers.Interfaces
{
    class Refresh : Controller, Executable
    {
        public void Execute() => Router.Interfaces.Instance.Refresh();

        public object Export() => new Table().Export();
    }
}