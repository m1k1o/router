using Router.Helpers;
using System;
using System.Net;

namespace Router.Controllers.Interfaces
{
    class Edit : Controller, Executable
    {
        public Interface ID { get; set; }
        public IPAddress IP { get; set; }
        public IPSubnetMask Mask { get; set; }

        public void Execute()
        {
            if (ID == null || IP == null || Mask == null)
            {
                throw new Exception("Expected Interface, IP, Mask.");
            }

            ID.SetIP(IP, Mask);
        }

        public object Export() => this;
    }
}
