using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public JSON Show(string Data)
        {
            var arr = new JSONArray();
            var obj = new JSONObject();

            var Interfaces = Instance.GetInteraces();

            int i = 0;
            foreach(var Interface in Interfaces)
            {
                obj.Empty();

                obj.Push("id", i);
                obj.Push("name", Interface.Name);
                obj.Push("description", Interface.Description);
                obj.Push("selected", Interface.Selected);

                arr.Push(obj);
                i++;
            }

            return arr;
        }
    }
}
