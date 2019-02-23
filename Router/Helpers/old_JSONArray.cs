using System.Collections.Generic;

namespace Router.Helpers
{
    class old_JSONArray : old_JSON
    {
        public old_JSONArray(List<old_JSON> List = null)
        {
            if (List != null)
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
