using System;
using System.Collections.Generic;

namespace Router.Packets
{
    static class Generator
    {
        public static void AutoTypes(IGeneratorPacket Packet, IGeneratorPacket LastPacket)
        {
            if (Packet is Ethernet)
            {
                if (LastPacket is IP)
                {
                    ((Ethernet)Packet).EthernetPacketType = IP.EthernetPacketType;
                }
            }

            if (Packet is IP)
            {
                if (LastPacket is ICMP)
                {
                    ((IP)Packet).IPProtocolType = ICMP.IPProtocolType;
                }

                if (LastPacket is UDP)
                {
                    ((IP)Packet).IPProtocolType = UDP.IPProtocolType;
                }

                if (LastPacket is TCP)
                {
                    ((IP)Packet).IPProtocolType = TCP.IPProtocolType;
                }
            }
        }

        public static byte[] Export(List<IGeneratorPacket> Packets)
        {
            Packets.Reverse();

            var LastPacket = (IGeneratorPacket)null;
            foreach (var Packet in Packets)
            {
                if (LastPacket == null)
                {
                    LastPacket = Packet;
                    continue;
                }

                if (Packet is IGeneratorPayload)
                {
                    ((IGeneratorPayload)Packet).PayloadData(LastPacket.Export());
                    AutoTypes(Packet, LastPacket);
                    LastPacket = Packet;
                }
                else
                {
                    throw new Exception(Packet + " is not Payload packet.");
                }
            }

            return LastPacket.Export();
        }
        public static List<IGeneratorPacket> Export(byte[] Data)
        {
            throw new NotImplementedException();
        }
    }

    interface IGeneratorPayload
    {
        void PayloadData(byte [] Data);
    }

    interface IGeneratorPacket
    {
        byte[] Export();

        void Import(byte[] Bytes);
    }
}
