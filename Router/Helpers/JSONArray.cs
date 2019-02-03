namespace Router.Helpers
{
    class JSONArray
    {
        private string Data = "";

        public void push(string value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(value);
        }

        public void push(int value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value.ToString();
        }

        public void push(bool value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value ? "true" : "false";
        }

        public void push(JSONArray value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value.ToString();
        }

        public void push(JSONObject value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value.ToString();
        }

        public override string ToString()
        {
            return "[" + Data + "]";
        }
    }
}
