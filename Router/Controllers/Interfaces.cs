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

        public JSON Start(string Data)
        {
            // Validate
            Interface Interface;
            try
            {
                Interface = Instance.GetInterfaceById(Data);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            if (Interface.IPNetwork == null)
            {
                return new JSONError("You must first set IPAddress and IPSubnetMask.");
            }

            if (!Interface.Running)
            {
                Interface.Start();
            }

            // Answer
            return new JSONObject("running", Interface.Running);
        }

        public JSON Stop(string Data)
        {
            // Validate
            Interface Interface;
            try
            {
                Interface = Instance.GetInterfaceById(Data);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            if (Interface.Running)
            {
                Interface.Stop();
            }

            // Answer
            return new JSONObject("running", Interface.Running);
        }
        
        public JSON Edit(string Data)
        {
            var Rows = Data.Split('\n');
            if (Rows.Length != 3)
            {
                return new JSONError("Expected InterfaceID, IPAddress, Mask.");
            }

            Interface Interface;
            IPAddress IP;
            IPSubnetMask Mask;
            try
            {
                Interface = Instance.GetInterfaceById(Rows[0]);
                IP = IPAddress.Parse(Rows[1]);
                Mask = IPSubnetMask.Parse(Rows[2]);

                // Save
                Interface.SetIP(IP, Mask);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            return this.Interface(Interface);
        }

        public JSON Show(string Data = null)
        {
            var obj = new JSONObject();

            var Interfaces = Instance.GetInteraces();
            foreach(var Iface in Interfaces)
            {
                obj.Push(Iface.ToString(), Interface(Iface));
            }

            return obj;
        }

        public JSON Get(string Data = null)
        {
            Interface Interface;
            try
            {
                Interface = Instance.GetInterfaceById(Data);
            }
            catch (Exception e)
            {
                return new JSONError(e.Message);
            }

            return this.Interface(Interface);
        }
    }
}
