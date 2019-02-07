using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Router.Helpers
{
    class JSON
    {
        protected string Data = "";

        public JSON()
        {

        }

        public JSON(object value)
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

            if (value is JSONObject)
            {
                return ((JSONObject)value).ToString();
            }

            if (value is JSONArray)
            {
                return ((JSONArray)value).ToString();
            }

            if (value is int || value is JSON)
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
