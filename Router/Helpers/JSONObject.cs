namespace Router.Helpers
{
    class JSONObject
    {
        private string Data = "";

        public void push(string key, string value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + JSON.Escape(value);
        }

        public void push(string key, int value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + value.ToString();
        }

        public void push(string key, bool value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + (value ? "true" : "false");
        }

        public void push(string key, JSONObject value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + value.ToString();
        }

        public void push(string key, JSONArray value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + value.ToString();
        }

        public override string ToString()
        {
            return "{" + Data + "}";
        }
    }
}
