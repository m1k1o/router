namespace Router.Helpers
{
    class JSONArray
    {
        private string Data = "";

        public void Push(string value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += JSON.Escape(value);
        }

        public void Push(int value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value.ToString();
        }

        public void Push(bool value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value ? "true" : "false";
        }

        public void Push(JSONArray value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value.ToString();
        }

        public void Push(JSONObject value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += value.ToString();
        }

        public void Empty()
        {
            Data = "";
        }

        public override string ToString()
        {
            return "[" + Data + "]";
        }
    }
}
