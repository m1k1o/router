using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Router.Protocols.DHCPOptions
{
    class DHCPMessageTypeOption : DHCPOption
    {
        [JsonConverter(typeof(StringEnumConverter))] // Serialize enums by name rather than numerical value
        public DHCPMessageType MessageType { get; set; }

        public DHCPMessageTypeOption(byte[] Bytes) : base(DHCPOptionCode.MessageType)
        {
            MessageType = (DHCPMessageType)Bytes[0];
        }

        public DHCPMessageTypeOption(DHCPMessageType MessageType) : base(DHCPOptionCode.MessageType)
        {
            this.MessageType = MessageType;
        }

        public override byte[] Bytes => new byte[] { (byte)MessageType };
    }

    enum DHCPMessageType : byte
    {
        // DHCP Discover message
        Discover = 1,
        
        // DHCP Offer message
        Offer = 2,

        // DHCP Request message
        Request = 3,
        
        // DHCP Decline message
        Decline = 4,
        
        // DHCP Acknowledgment message
        Ack = 5,
        
        // DHCP Negative Acknowledgment message
        Nak = 6,
        
        // DHCP Release message
        Release = 7
    }
}
