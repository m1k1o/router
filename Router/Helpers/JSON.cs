namespace Router.Helpers
{
    class JSON
    {
        static public string Escape(string Str)
        {
            Str = Str.Replace(System.Environment.NewLine, "\\n");
            return "\"" + Str + "\"";
        }
    }
}
