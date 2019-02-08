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
            PossibblyDown = false;
        }

        public bool NeverUpdated {
            get => TimeUpdated == DateTime.MinValue;
        }

        public bool ToBeRemoved {
            get => DateTime.Now > TimeUpdated + FlushTimer;
        }

        private DateTime PossibblyDownSince;

        public bool PossibblyDown
        {
            get =>
                PossibblyDownSince != DateTime.MinValue ||
                DateTime.Now > TimeUpdated + InvalidTimer;
            set
            {
                if (!value)
                {
                    PossibblyDownSince = DateTime.Now;
                }
                else
                {
                    PossibblyDownSince = DateTime.MinValue;
                }
            }
        }

        public bool InHold
        {
            get =>
                (PossibblyDownSince != DateTime.MinValue && DateTime.Now <= PossibblyDownSince + HoldTimer) ||
                (PossibblyDownSince == DateTime.MinValue && DateTime.Now > TimeUpdated + InvalidTimer && DateTime.Now <= TimeUpdated + InvalidTimer + HoldTimer);
        }
    }
}
