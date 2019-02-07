using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.RIP
{
    abstract class RIPTimers
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
        }

        public bool NeverUpdated {
            get => TimeUpdated == DateTime.MinValue;
        }

        public bool ToBeRemoved {
            get => DateTime.Now > TimeUpdated + FlushTimer;
        }

        public bool Valid {
            get => DateTime.Now <= TimeUpdated + InvalidTimer;
        }

        private DateTime HoldSince;

        public bool InHold
        {
            get =>
                (HoldSince != DateTime.MinValue && DateTime.Now <= HoldSince + HoldTimer) ||
                (!Valid && DateTime.Now <= TimeUpdated + InvalidTimer + HoldTimer);
            set
            {
                if (value)
                {
                    HoldSince = DateTime.Now;
                }
                else
                {
                    HoldSince = DateTime.MinValue;
                }
            }
        }
    }
}
