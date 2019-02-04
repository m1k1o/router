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

        public bool Running(string Data)
        {
            return Instance.Running;
        }

        public JSON Start(string Data)
        {
            if (Instance.Running)
            {
                return new JSONError("Router is already running.");
            }

            Instance.Open();
            return new JSON(true);
        }

        public JSON Stop(string Data)
        {
            if (!Instance.Running)
            {
                return new JSONError("Router is not running.");
            }

            Instance.Close();
            return new JSON(true);
        }

        public JSON Select(string Data)
        {
            if (Instance.Running)
            {
                return new JSONError("Router is already running.");
            }

            if (string.IsNullOrEmpty(Data))
            {
                return new JSONError("No ID specified.");
            }

            var ID = Int32.Parse(Data);
            Instance.SelectInterface(ID);
            return new JSON(true);
        }
        
        public JSON Unselect(string Data)
        {
            if (Instance.Running)
            {
                return new JSONError("Router is already running.");
            }

            if (string.IsNullOrEmpty(Data))
            {
                return new JSONError("No ID specified.");
            }

            var ID = Int32.Parse(Data);
            Instance.UnselectInterface(ID);
            return new JSON(true);
        }

        private JSON Interface(Interface Interface)
        {
            var obj = new JSONObject();
            obj.Push("id", Instance.GetInteraces().IndexOf(Interface));
            obj.Push("name", Interface.Name);
            obj.Push("friendly_name", Interface.FriendlyName);
            obj.Push("description", Interface.Description);
            obj.Push("selected", Interface.Selected);
            obj.Push("ip", Interface.IPAddress);
            obj.Push("mask", Interface.Mask);
            obj.Push("mac", Interface.PhysicalAddress);
            return obj;
        }

        public JSON Edit(string Data)
        {
            var Rows = Data.Split('\n');

            // Validate
            if (Rows.Length != 4)
            {
                return new JSONError("Expected 4 pieces of data.");
            }

            IPAddress IP;
            IPAddress Mask;
            try
            {
                IP = IPAddress.Parse(Rows[2]);
                Mask = IPAddress.Parse(Rows[3]);
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
            iface.Selected = (Rows[1] == "true");
            
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
    }
}
