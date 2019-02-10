using Router.Helpers;
using System;
using System.Net;

namespace Router.Controllers
{
    static class Interfaces
    {
        private static readonly Router.Interfaces Instance = Router.Interfaces.Instance;

        private static JSON Interface(Interface Interface)
        {
            var obj = new JSONObject();
            //obj.Push("id", Interface.ID);
            obj.Push("name", Interface.Name);
            obj.Push("friendly_name", Interface.FriendlyName);
            obj.Push("description", Interface.Description);
            obj.Push("running", Interface.Running);
            obj.Push("ip", Interface.IPAddress);
            obj.Push("mask", Interface.IPNetwork is IPNetwork ? Interface.IPNetwork.SubnetMask : null);
            obj.Push("mac", Interface.PhysicalAddress);
            return obj;
        }

        public static JSON Toggle(string Data)
        {
            try
            {
                var Interface = Instance.GetInterfaceById(Data);
                if (!Interface.Running)
                {
                    Interface.Start();
                }
                else
                {
                    Interface.Stop();
                }

                return new JSONObject("running", Interface.Running);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }
        }

        public static JSON Edit(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 3)
            {
                return new JSONError("Expected InterfaceID, IPAddress, IPSubnetMask.");
            }

            Interface Interface;
            try
            {
                Interface = Instance.GetInterfaceById(Rows[0]);

                var IP = IPAddress.Parse(Rows[1]);
                var SubnetMask = IPSubnetMask.Parse(Rows[2]);
                Interface.SetIP(IP, SubnetMask);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            return Interfaces.Interface(Interface);
        }
        
        public static JSON Table(string Data = null)
        {
            var obj = new JSONObject();

            var Interfaces = Instance.GetInteraces();
            foreach(var Iface in Interfaces)
            {
                obj.Push(Iface.ID.ToString(), Interface(Iface));
            }

            return obj;
        }

        public static JSON Initialize(string Data = null)
        {
            var obj = new JSONObject();
            obj.Push("table", Table());
            return obj;
        }
    }
}
