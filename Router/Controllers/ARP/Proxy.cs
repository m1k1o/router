using Router.ARP;

namespace Router.Controllers.ARP
{
    class Proxy : Controller
    {
        public bool Enabled
        {
            get => ARPMiddleware.ProxyEnabled;
            set => ARPMiddleware.ProxyEnabled = value;
        }

        public object Export() => this;
    }
}
