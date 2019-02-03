namespace Router.Helpers
{
    class JSONError : JSONObject
    {
        public JSONError(object msg) : base("error", msg)
        {

        }
    }
}
