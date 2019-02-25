using Newtonsoft.Json;
using Router.Helpers;
using System;
using System.Collections.Generic;

namespace Router.Packets
{
    static class Generator
    {
        public static IGeneratorPacket Factory(string PacketType)
        {
            switch (PacketType)
            {
                case "Ethernet":
                    return new Ethernet();
                case "ARP":
                    return new ARP();
                case "IP":
                    return new IP();

                case "ICMP":
                    return new ICMP();
                case "TCP":
                    return new TCP();
                case "UDP":
                    return new UDP();

                case "DHCP":
                    return new DHCP();
                case "RIP":
                    return new RIP();

                default:
                    throw new ArgumentException("Invalid Packet Type");
            }

        }

        public static void AutoTypes(IGeneratorPacket Packet, IGeneratorPacket LastPacket)
        {
            /*
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
            */
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

        public static string ExportJSON(List<IGeneratorPacket> Packets)
        {
            var Serialize = new List<KeyValuePair<string, IGeneratorPacket>>();
            foreach (var Packet in Packets)
            {
                Serialize.Add(new KeyValuePair<string, IGeneratorPacket>(Packet.GetType().Name, Packet));
            }
            return JSON.SerializeObject(Serialize);
        }

        public static List<IGeneratorPacket> Import(byte[] Data)
        {
            throw new NotImplementedException();
        }

        public static List<IGeneratorPacket> ImportJSON(string json)
        {
            var Deserialize = new List<IGeneratorPacket>();
            JSON.PopulateObject(json, Deserialize);
            return Deserialize;
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
