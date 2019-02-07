using Router.Helpers;

namespace Router.Protocols
{
    public sealed class RIPPacket : Packet
    {
        public byte CommandType
        {
            get => (byte)Slice(0, typeof(byte));
            set => Inject(0, value);
        }

        public byte Version
        {
            get => (byte)Slice(1, typeof(byte));
            set => Inject(1, value);
        }

        public ushort MustBeZero
        {
            get => (ushort)Slice(2, typeof(ushort));
            set => Inject(2, value);
        }

        public RIPRouteCollection RouteCollection
        {
            get
            {
                var offset = 4;
                var newColection = new RIPRouteCollection();
                do {
                    var RTEBytes = Slice(offset, RIPRoute.Length);
                    newColection.Add(new RIPRoute(RTEBytes));
                    offset += RIPRoute.Length;
                } while (offset < Length);

                return newColection;
            }

            set
            {
                var offset = 4;
                foreach (RIPRoute item in value)
                {
                    Inject(offset, item.Bytes, item.Bytes.Length);
                    offset += item.Bytes.Length;
                }
            }
        }

        public RIPPacket(RIPCommandType CommandType, RIPRouteCollection RouteCollection) : base(4)
        {
            this.CommandType = (byte)CommandType;
            Version = 2;
            MustBeZero = 0;
            this.RouteCollection = RouteCollection;
        }

        public RIPPacket(byte[] Data) : base(Data)
        {

        }

        public override string ToString()
        {
            return "RIP " + CommandType + "\n" + RouteCollection.ToString();
        }
    }
}