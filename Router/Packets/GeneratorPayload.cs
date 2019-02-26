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

                return PayloadData;
            }
            set
            {
                PayloadData = value;
                PayloadPacket = null;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] PayloadData { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public GeneratorPacket PayloadPacket { get; set; }

        public GeneratorPayload(byte[] Bytes)
        {
            PayloadData = Bytes;
        }

        public GeneratorPayload(GeneratorPacket Packet)
        {
            PayloadPacket = Packet;
        }

        public GeneratorPayload() { }
    }
}
