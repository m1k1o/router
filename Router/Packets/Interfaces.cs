using System;
using System.Collections.Generic;

namespace Router.Packets
{
    static class Generator
    {
        public static byte[] Parse(List<PacketsImportExport> Packets)
        {
            Packets.Reverse();

            var LastPacket = (PacketsImportExport)null;
            foreach (var Packet in Packets)
            {
                if (LastPacket == null)
                {
                    LastPacket = Packet;
                    continue;
                }

                if (Packet is PacketsPayloadData)
                {
                    ((PacketsPayloadData)Packet).PayloadData(LastPacket.Export());
                    LastPacket = Packet;
                }
                else
                {
                    throw new Exception(Packet + " is not Payload packet.");
                }
            }

            return LastPacket.Export();
        }
    }
    interface PacketsPayloadData
    {
        void PayloadData(byte [] Data);
    }

    interface PacketsImportExport
    {
        byte[] Export();

        void Import(byte[] Bytes);
    }
}
