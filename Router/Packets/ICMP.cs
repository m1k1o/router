using PacketDotNet;
using PacketDotNet.Utils;

namespace Router.Packets
{
    sealed class ICMP : GeneratorPayload
    {
        public ICMPv4TypeCodes TypeCode { get; set; }

        public ushort ID { get; set; } = 0;
        public ushort Sequence { get; set; } = 0;

        public ICMP() { }

        public override byte[] Export()
        {
            var ICMPPacket = new ICMPv4Packet(new ByteArraySegment(new byte[ICMPv4Fields.HeaderLength]))
            {
                TypeCode = TypeCode,
                Checksum = 0,
                ID = ID,
                Sequence = Sequence
            };

            if (Payload != null)
            {
                ICMPPacket.Data = Payload;
            }

            ICMPPacket.Checksum = (ushort)ChecksumUtils.OnesComplementSum(ICMPPacket.Bytes);
            return ICMPPacket.Bytes;
        }

        public override void Import(byte[] Bytes)
        {
            if (Bytes == null) return;

            var ICMPv4Packet = new ICMPv4Packet(new ByteArraySegment(Bytes));

            TypeCode = ICMPv4Packet.TypeCode;
            ID = ICMPv4Packet.ID;

            // Some Types contains IP Header
            var Type = (byte)((ushort)TypeCode >> 8);
            switch (Type)
            {
                case 3:
                case 5:
                case 11:
                case 12:
                    PayloadPacket = new IP();
                    PayloadPacket.Import(ICMPv4Packet.Data);
                    break;
                default:
                    Payload = ICMPv4Packet.Data;
                    break;
            }
        }
    }

    /*
    enum ICMPTypeCodes : ushort
    {
        Echo_Reply = 0,

        //Destination_Unreachable
        Destination_Unreachable_Net_Unreachable = 768,
        Destination_Unreachable_Host_Unreachable = 769,
        Destination_Unreachable_Protocol_Unreachable = 770,
        Destination_Unreachable_Port_Unreachable = 771,
        Destination_Unreachable_Fragmentation_Needed_and_Dont_Fragment_was_Set = 772,
        Destination_Unreachable_Source_Route_Failed = 773,
        Destination_Unreachable_Destination_Network_Unknown = 774,
        Destination_Unreachable_Destination_Host_Unknown = 775,
        Destination_Unreachable_Source_Host_Isolated = 776,
        Destination_Unreachable_Communication_with_Destination_Network_is_Administratively_Prohibited = 777,
        Destination_Unreachable_Communication_with_Destination_Host_is_Administratively_Prohibited = 778,
        Destination_Unreachable_Destination_Network_Unreachable_for_Type_of_Service = 779,
        Destination_Unreachable_Destination_Host_Unreachable_for_Type_of_Service = 780,
        Destination_Unreachable_Communication_Administratively_Prohibited = 781,
        Destination_Unreachable_Host_Precedence_Violation = 782,
        Destination_Unreachable_Precedence_cutoff_in_effect = 783,

        //Redirect
        Redirect_Datagram_for_the_Network = 1280,
        Redirect_Datagram_for_the_Host = 1281,
        Redirect_Datagram_for_the_Type_of_Service_and_Network = 1282,
        Redirect_Datagram_for_the_Type_of_Service_and_Host = 1283,

        //_MISC
        Echo = 2048,
        Router_Advertisement = 2304,
        Router_Solicitation = 2560,
        Time_to_Live_exceeded_in_Transit = 2816,

        //Parameter_Problem
        Parameter_Problem_Pointer_indicates_the_error = 3072,
        Parameter_Problem_Missing_a_Required_Option = 3073,
        Parameter_Problem_Bad_Length = 3074,

        //_MISC
        Timestamp = 3328,
        Timestamp_Reply = 3584,

        //Photuris
        Photuris_Bad_SPI = 9728,
        Photuris_Authentication_Failed = 9729,
        Photuris_Decompression_Failed = 9730,
        Photuris_Decryption_Failed = 9731,
        Photuris_Need_Authentication = 9732,
        Photuris_Need_Authorization = 9733
    }
    */
}
