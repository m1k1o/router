﻿using Router.Helpers;
using System;
using System.Net;

namespace Router.RIP
{
    class RIPEntry : RIPEntryTimers
    {
        public int ID => GetHashCode();

        public Interface Interface { get; private set; }
        public IPNetwork IPNetwork { get; private set; }
        public IPAddress NextHopIP { get; private set; }

        private uint metric;
        public uint Metric {
            get => metric;
            private set {
                if (value < 0 || value > 15)
                {
                    throw new Exception("Invalid Metric, use number from interval <1;15>");
                }

                metric = value;
            }
        }

        public bool SyncWithRT { get; set; } = true;
        public bool CanBeUpdated { get; set; } = true;

        public RIPEntry(Interface Interface, IPNetwork IPNetwork, IPAddress NextHopIP, uint Metric)
        {
            this.Interface = Interface;
            this.IPNetwork = IPNetwork;
            this.NextHopIP = NextHopIP;
            this.Metric = Metric;
            Create();
        }

        public bool Update(IPAddress NextHopIP, uint Metric)
        {
            if (!CanBeUpdated) return false;

            var HasChanged = !Equals(this.NextHopIP, NextHopIP) || this.Metric != Metric;
            if (HasChanged)
            {
                this.NextHopIP = NextHopIP;
                this.Metric = Metric;
            }

            Update();
            SyncWithRT = true;
            return HasChanged;
        }

        public override string ToString()
        {
            return
                "RIPEntry:" + IPNetwork.ToString() + " via " + NextHopIP.ToString() + ":\n" +
                "\tInterface:\t" + Interface.ToString() + "\n" +
                "\tMetric:\t" + Metric.ToString() + "\n" +

                "\tSyncWithRT:\t" + SyncWithRT.ToString() + "\n" +
                "\tCanBeUpdated:\t" + CanBeUpdated.ToString() + "\n" +
                "\t\tNeverUpdated:\t" + NeverUpdated.ToString() + "\n" +

                "\tTimersEnabled:\t" + TimersEnabled.ToString() + "\n" +
                "\t\tPossibblyDown:\t" + PossibblyDown.ToString() + "\n" +
                "\t\tInHold:\t" + InHold.ToString() + "\n" +
                "\t\tToBeRemoved:\t" + ToBeRemoved.ToString() + "\n";
        }

        // Equality

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = IPNetwork.GetHashCode();
                hashCode = (hashCode * 397) ^ Interface.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(RIPEntry RIPEntry)
            => !(RIPEntry is null) && Equals(RIPEntry.IPNetwork, IPNetwork) && Equals(RIPEntry.Interface, Interface);

        public override bool Equals(object obj) =>
            !(obj is null) && obj.GetType() == GetType() && Equals(obj as RIPEntry);
    }
}
