namespace Router.Helpers
{
    class old_JSONObject : old_JSON
    {
        public old_JSONObject(string Key = null, object Value = null)
        {
            if (Key != null)
            {
                Add(Key, Value);
            }
        }

        public void Add(string key, object value)
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
