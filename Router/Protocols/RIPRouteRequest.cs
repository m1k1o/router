namespace Router.Protocols
{
    class RIPRouteRequest : RIPRoute
    {
        public RIPRouteRequest() : base()
        {
            Metric = 16;
        }
    }
}
