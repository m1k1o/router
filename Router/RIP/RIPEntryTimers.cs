using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    abstract class RIPEntryTimers
    {
        static public TimeSpan InvalidTimer = TimeSpan.FromSeconds(180);
        static public TimeSpan HoldTimer = TimeSpan.FromSeconds(180);
        static public TimeSpan FlushTimer = TimeSpan.FromSeconds(240);

        private DateTime TimeCreated;
        private DateTime TimeUpdated;

        protected void Create()
        {
            TimeCreated = DateTime.Now;
        }

        protected void Update()
        {
            TimeUpdated = DateTime.Now;
            Valid = true;
        }

        public bool NeverUpdated {
            get => TimeUpdated == DateTime.MinValue;
        }

        public bool ToBeRemoved {
            get => DateTime.Now > TimeUpdated + FlushTimer;
        }

        private DateTime InvalidSince;

        public bool Valid {
            get =>
                InvalidSince != DateTime.MinValue ||
                DateTime.Now <= TimeUpdated + InvalidTimer;
            set
            {
                if (!value)
                {
                    InvalidSince = DateTime.Now;
                }
                else
                {
                    InvalidSince = DateTime.MinValue;
                }
            }
        }

        public bool InHold
        {
            get =>
                (InvalidSince != DateTime.MinValue && DateTime.Now > InvalidSince + HoldTimer) ||
                (!Valid && DateTime.Now > TimeUpdated + InvalidTimer + HoldTimer);
        }
    }
}
