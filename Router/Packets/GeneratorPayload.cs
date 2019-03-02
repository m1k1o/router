using Newtonsoft.Json;

namespace Router.Packets
{
    abstract class GeneratorPayload : GeneratorPacket
    {
        [JsonIgnore]
        public byte[] Payload
        {
            get
            {
                if (PayloadPacket != null)
                {
                    return PayloadPacket.Export();
                }

                return null;
            }
            set
            {
                PayloadPacket = new Payload();
                PayloadPacket.Import(value);
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public GeneratorPacket PayloadPacket { get; set; }

        public GeneratorPayload(byte[] Bytes)
        {
            PayloadPacket = new Payload();
            PayloadPacket.Import(Bytes);
        }

        public GeneratorPayload(GeneratorPacket Packet)
        {
            PayloadPacket = Packet;
        }

        public GeneratorPayload() { }
    }
}
