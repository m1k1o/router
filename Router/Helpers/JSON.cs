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
