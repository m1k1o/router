using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Router
{
    class RIPEntry
    {
        static public TimeSpan InvalidTimer = TimeSpan.FromSeconds(180);
        static public TimeSpan HoldTimer = TimeSpan.FromSeconds(180);
        static public TimeSpan FlushTimer = TimeSpan.FromSeconds(240);

        public Interface Interface;
        public IPNetwork IPNetwork;
        public IPAddress NextHopIP;
        public uint Metric;

        private DateTime TimeCreated;
        private DateTime TimeUpdated;

        public RIPEntry(Interface Interface, IPNetwork IPNetwork, IPAddress NextHopIP, uint Metric)
        {
            this.Interface = Interface;
            this.IPNetwork = IPNetwork;
            this.NextHopIP = NextHopIP;
            this.Metric = Metric;

            TimeCreated = DateTime.Now;
        }

        public bool Update(IPAddress NextHopIP, uint Metric)
        {
            var HasChanged = this.NextHopIP != NextHopIP || this.Metric != Metric;
            if (HasChanged)
            {
                this.NextHopIP = NextHopIP;
                this.Metric = Metric;
            }

            TimeUpdated = DateTime.Now;
            return HasChanged;
        }

        public bool NeverUpdated { get => TimeUpdated == DateTime.MinValue; }

        public bool ToBeRemoved { get => DateTime.Now > TimeUpdated + FlushTimer; }

        public bool Valid { get => DateTime.Now <= TimeUpdated + InvalidTimer; }

        private bool forceHold;

        public bool InHold {
            get => 
                (forceHold && DateTime.Now <= TimeUpdated + HoldTimer ) || 
                (!Valid && DateTime.Now <= TimeUpdated + InvalidTimer + HoldTimer);
            set => forceHold = value;
        }
    }
}
