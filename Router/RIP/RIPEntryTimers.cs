using System;

namespace Router.RIP
{
    abstract class RIPEntryTimers
    {
        static public TimeSpan InvalidTimer = TimeSpan.FromSeconds(180);
        static public TimeSpan HoldTimer = TimeSpan.FromSeconds(180);
        static public TimeSpan FlushTimer = TimeSpan.FromSeconds(240);

        private DateTime TimeCreated;
        private DateTime TimeUpdated;
        private DateTime PossibblyDownSince;

        protected void Create()
        {
            TimeCreated = DateTime.Now;
            TimeUpdated = DateTime.Now;
        }

        protected void Update()
        {
            TimeUpdated = DateTime.Now;
            PossibblyDown = false;
        }

        public bool NeverUpdated {
            get => TimeUpdated == TimeCreated;
        }

        public bool CanBeRemoved { get; set; } = true;

        public bool ToBeRemoved {
            get => CanBeRemoved && DateTime.Now > TimeUpdated + FlushTimer;
        }

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
