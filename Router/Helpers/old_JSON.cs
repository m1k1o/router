using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Router.Helpers
{
    class old_JSON
    {
        protected string Data = "";

        public old_JSON()
        {

        }

        public old_JSON(object value)
        {
            Data += Escape(value);
        }

        public string Escape(string value)
        {
            value = value.Replace(System.Environment.NewLine, "\\n");
            value = value.Replace("\\", "\\\\");
            return "\"" + value + "\"";
        }

        public string Escape(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is bool)
            {
                return ((bool)value) ? "true" : "false";
            }

            if (value is old_JSONObject)
            {
                return ((old_JSONObject)value).ToString();
            }

            if (value is old_JSONArray)
            {
                return ((old_JSONArray)value).ToString();
            }

            if (value is double || value is float || value is int || value is old_JSON || value is Newtonsoft.Json.Linq.JToken)
            {
                return value.ToString();
            }

            if (value is PhysicalAddress)
            {
                var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                var replace = "$1:$2:$3:$4:$5:$6";
                return "\"" + Regex.Replace(value.ToString(), regex, replace) + "\"";
            }

            return Escape(value.ToString());
        }

        public void Empty()
        {
            Data = "";
        }

        public override string ToString()
        {
            return Data;
        }
    }
}
