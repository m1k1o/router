namespace Router.Generator
{
    interface Generator
    {
        byte[] Export();
        byte[] ExportAll();

        //void Parse(string[] Rows, ref int i);
    }
}
