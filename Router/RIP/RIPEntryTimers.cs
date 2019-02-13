using System;

namespace Router.RIP
{
    abstract class RIPEntryTimers
    {
        public static TimeSpan InvalidTimer = TimeSpan.FromSeconds(180);
        public static TimeSpan HoldTimer = TimeSpan.FromSeconds(180);
        public static TimeSpan FlushTimer = TimeSpan.FromSeconds(240);

        private DateTime TimeCreated;
        private DateTime TimeUpdated;
        private DateTime PossibblyDownSince;
        
        public int SinceLastUpdate { get => (int)(DateTime.Now - TimeUpdated).TotalSeconds; }

        protected void Create()
        {
            TimeCreated = DateTime.Now;
            TimeUpdated = TimeCreated;
        }

        protected void Update()
        {
            TimeUpdated = DateTime.Now;
            PossibblyDown = false;
        }

        public bool NeverUpdated {
            get => TimeUpdated == TimeCreated;
        }

        public bool TimersEnabled { get; set; } = true;

        public bool ToBeRemoved {
            get => 
                TimersEnabled &&
                ((PossibblyDownSince != DateTime.MinValue && DateTime.Now > PossibblyDownSince + FlushTimer) ||
                (PossibblyDownSince == DateTime.MinValue && DateTime.Now > TimeUpdated + FlushTimer));
        }

        public bool PossibblyDown
        {
            get =>
                PossibblyDownSince != DateTime.MinValue ||
                (TimersEnabled && DateTime.Now > TimeUpdated + InvalidTimer);
            set
            {
                if (value && PossibblyDownSince == DateTime.MinValue)
                {
                    PossibblyDownSince = DateTime.Now - InvalidTimer;
                }

                if (!value) {
                    PossibblyDownSince = DateTime.MinValue;
                }
            }
        }

        public bool InHold
        {
            get =>
                TimersEnabled &&
                ((PossibblyDownSince != DateTime.MinValue && DateTime.Now <= PossibblyDownSince + InvalidTimer + HoldTimer) ||
                (PossibblyDownSince == DateTime.MinValue && DateTime.Now > TimeUpdated + InvalidTimer && DateTime.Now <= TimeUpdated + InvalidTimer + HoldTimer));
        }
    }
}
