namespace Router.Helpers
{
    class JSONObject : JSON
    {
        public JSONObject(string Key = null, object Value = null)
        {
            if (Key != null)
            {
                Push(Key, Value);
            }
        }

        public void Push(string key, object value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += Escape(key) + ":" + Escape(value);
        }

        public override string ToString()
        {
            return "{" + Data + "}";
        }
    }
}
