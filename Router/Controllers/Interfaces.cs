using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router.Controllers
{
    class Interfaces
    {
        static private readonly Router.Interfaces Instance = Router.Interfaces.Instance;

        private JSON Interface(Interface Interface)
        {
            var obj = new JSONObject();
            obj.Push("id", Instance.GetInteraces().IndexOf(Interface));
            obj.Push("name", Interface.Name);
            obj.Push("friendly_name", Interface.FriendlyName);
            obj.Push("description", Interface.Description);
            obj.Push("selected", Interface.Running);
            obj.Push("ip", Interface.IPAddress);
            obj.Push("mask", Interface.Mask);
            obj.Push("mac", Interface.PhysicalAddress);
            return obj;
        }

        public JSON Start(string Data)
        {
            if (string.IsNullOrEmpty(Data))
            {
                return new JSONError("No ID specified.");
            }

            var ID = Int32.Parse(Data);
            var Iface = Instance.GetInterfaceById(ID);

            if (Iface.IPAddress == null || Iface.Mask == null)
            {
                return new JSONError("You must first set IP and Mask.");
            }

            if (!Iface.Running)
            {
                Iface.Start();
            }

            return new JSONObject("running", Iface.Running);
        }

        public JSON Stop(string Data)
        {
            if (string.IsNullOrEmpty(Data))
            {
                return new JSONError("No ID specified.");
            }

            var ID = Int32.Parse(Data);
            var Iface = Instance.GetInterfaceById(ID);

            if (Iface.Running)
            {
                Iface.Stop();
            }

            return new JSONObject("running", Iface.Running);
        }
        
        public JSON Edit(string Data)
        {
            var Rows = Data.Split('\n');

            // Validate
            if (Rows.Length != 3)
            {
                return new JSONError("Expected InterfaceID, IPAddress, Mask.");
            }

            IPAddress IP;
            IPAddress Mask;
            try
            {
                IP = IPAddress.Parse(Rows[1]);
                Mask = IPAddress.Parse(Rows[2]);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            // Save
            var ID = Int32.Parse(Rows[0]);
            var iface = Instance.GetInterfaceById(ID);
            iface.IPAddress = IP;
            iface.Mask = Mask;
            
            return Interface(iface);
        }

        public JSON Show(string Data)
        {
            var arr = new JSONArray();

            var Interfaces = Instance.GetInteraces();
            foreach(var Iface in Interfaces)
            {
                arr.Push(Interface(Iface));
            }

            return arr;
        }

        public JSON Get(string Data)
        {
            if (string.IsNullOrEmpty(Data))
            {
                return new JSONError("No ID specified.");
            }

            var ID = Int32.Parse(Data);
            var Iface = Instance.GetInterfaceById(ID);

            return Interface(Iface);
        }
    }
}
