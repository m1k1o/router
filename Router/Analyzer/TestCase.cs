using System;
using System.Threading;

namespace Router.Analyzer
{
    abstract class TestCase
    {
        public TimeSpan Timeout { get; protected set; } = TimeSpan.FromSeconds(25);
        public bool Success { get; protected set; } = false;
        public bool Continue { get; protected set; } = true;
        public string Log { get; protected set; } = String.Empty;

        public bool Timeouted { get; private set; } = false;
        public bool Running { get; private set; } = false;

        abstract public void Generate(Interface Interface);
        abstract public void Analyze(Handler Handler);

        public void Execute(Interface GeneratorInterface, Interface AnalyzerInterface)
        {
            ManualResetEvent BlocingWaiting = new ManualResetEvent(false);

            void OnPacketArrival(Handler Handler)
            {
                if (Continue && Running)
                {
                    Analyze(Handler);
                }
                else
                {
                    BlocingWaiting.Set();
                }
            }

            AnalyzerInterface.OnPacketArrival += OnPacketArrival;

            Generate(GeneratorInterface);
            if(!BlocingWaiting.WaitOne(Timeout))
            {
                Timeouted = true;
            }

            Running = false;
            AnalyzerInterface.OnPacketArrival -= OnPacketArrival;
        }
    }
}
