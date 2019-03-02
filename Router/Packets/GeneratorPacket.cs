using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Router.Packets
{
    public enum GeneratorPackets
    {
        // Layer 2
        Ethernet,

        // Layer 3
        IP, ARP, ICMP,

        // Layer 4
        TCP, UDP,

        // Layer 7
        DHCP, RIP,

        // Data
        Payload
    }

    abstract class GeneratorPacket
    {
        static readonly Dictionary<Type, GeneratorPackets> TypeToGeneratorPackets;
        static readonly Dictionary<GeneratorPackets, Type> GeneratorPacketsToType;

        static GeneratorPacket()
        {
            TypeToGeneratorPackets = new Dictionary<Type, GeneratorPackets>()
            {
                { typeof(Ethernet), GeneratorPackets.Ethernet },

                { typeof(IP), GeneratorPackets.IP },
                { typeof(ARP), GeneratorPackets.ARP },
                { typeof(ICMP), GeneratorPackets.ICMP },

                { typeof(TCP), GeneratorPackets.TCP },
                { typeof(UDP), GeneratorPackets.UDP },

                { typeof(DHCP), GeneratorPackets.DHCP },
                { typeof(RIP), GeneratorPackets.RIP },

                { typeof(Payload), GeneratorPackets.Payload },
            };
            GeneratorPacketsToType = TypeToGeneratorPackets.ToDictionary(pair => pair.Value, pair => pair.Key);
        }

        public static Type GetType(GeneratorPackets GeneratorPacket) => GeneratorPacketsToType[GeneratorPacket];

        [JsonConverter(typeof(StringEnumConverter))] // Serialize enums by name rather than numerical value
        public GeneratorPackets type => TypeToGeneratorPackets[GetType()];

        public abstract byte[] Export();

        public abstract void Import(byte[] Bytes);
        
        public string ExportJSON() => JSON.SerializeObject(this);

        public void ImportJSON(string json) => JSON.PopulateObject(json, this);
    }
}
