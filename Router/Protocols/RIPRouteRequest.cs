namespace Router.Protocols
{
    class RIPRouteRequest : RIPRoute
    {
        public RIPRouteRequest() : base()
        {
            Metric = 16;
        }

        public override string ToString()
        {
            return "RIP Route Request";
        }
    }
}
