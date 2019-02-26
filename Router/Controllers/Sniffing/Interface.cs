using System;

namespace Router.Controllers.Sniffing
{
    class Interface : Controller
    {
        public Router.Interface ID
        {
            get => Router.Sniffing.SniffingList.Interface;
            set => Router.Sniffing.SniffingList.Interface = value;
        }

        public object Export() => this;
    }
}
