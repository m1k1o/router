using System.Collections.Generic;

namespace Router.Helpers
{
    class JSONArray : JSON
    {
        public JSONArray(List<object> List = null)
        {
            if(List != null)
            {
                foreach (var Row in List)
                {
                    Push(Row);
                }
            }
        }

        public void Push(object value)
        {
            if (Data != "")
            {
                Data += ",";
            }

            Data += Escape(value);
        }

        public override string ToString()
        {
            return "[" + Data + "]";
        }
    }
}
