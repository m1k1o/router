using System;

namespace Router.Controllers.Interfaces
{
    class Toggle : Controller, Executable
    {
        public Interface ID { get; set; }
        public bool? Running => ID?.Running;

        public void Execute()
        {
            if (ID == null)
            {
                throw new Exception("Expected Interface.");
            }

            if (!ID.Running)
            {
                ID.Start();
            }
            else
            {
                ID.Stop();
            }
        }

        public object Export()
        {
            return this;
        }
    }
}