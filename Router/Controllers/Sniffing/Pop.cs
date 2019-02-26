namespace Router.Controllers.Sniffing
{
    class Pop : Controller
    {
        public object Export() => Router.Sniffing.SniffingList.Pop();
    }
}
