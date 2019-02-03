namespace Router.Helpers
{
    class JSONObject
    {
        private string Data = "";

        public void Push(string key, string value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + JSON.Escape(value);
        }

        public void Push(string key, int value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + value.ToString();
        }

        public void Push(string key, bool value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + (value ? "true" : "false");
        }

        public void Push(string key, JSONObject value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + value.ToString();
        }

        public void Push(string key, JSONArray value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(key) + ":" + value.ToString();
        }

        public void Empty()
        {
            Data = "";
        }

        public override string ToString()
        {
            return "{" + Data + "}";
        }
    }
}
