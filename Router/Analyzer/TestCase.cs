using System;
using System.Threading;

namespace Router.Analyzer
{
    abstract class TestCase
    {
        public event Action OnStarted = delegate { };
        public event Action OnStopped = delegate { };
        public event Action<string> OnLogMessage = delegate { };

        const string NS_PREFIX = "Router.Analyzer.TestCases.";

        public Interface GeneratorInterface { get; set; }
        public Interface AnalyzerInterface { get; set; }

        abstract public string Name { get; }
        abstract public string Description { get; }

        abstract protected void Generate(Interface Interface);
        abstract protected void Analyze(Handler Handler);

        public TimeSpan Timeout { get; protected set; } = TimeSpan.FromSeconds(5);

        public TestCaseStatus Status { get; private set; } = TestCaseStatus.Idle;

        private ManualResetEvent BlocingWaiting = new ManualResetEvent(false);
        private Thread Thread;
        private Action Unsubscribe;

        private void Stop(TestCaseStatus Status)
        {
            if(this.Status == TestCaseStatus.Running)
            {
                this.Status = Status;
                BlocingWaiting.Set();

                Unsubscribe?.Invoke();
                OnStopped();
                Log("Test finished with status: " + Status);
            }
        }

        public void Start()
        {
            if(GeneratorInterface == null || AnalyzerInterface == null)
            {
                throw new Exception("No Interfaces set.");
            }

            if (!GeneratorInterface.Running || !AnalyzerInterface.Running)
            {
                throw new Exception("Interfaces must be running.");
            }

            // Started
            BlocingWaiting.Reset();
            Status = TestCaseStatus.Running;
            Log("Test '" + Name + "' started with timeout " + Timeout.TotalSeconds + " sec.");
            OnStarted();

            void OnPacketArrival(Handler Handler)
            {
                if (Status == TestCaseStatus.Running)
                {
                    Analyze(Handler);
                }
                else
                {
                    BlocingWaiting.Set();
                }
            }

            // Subscribe
            AnalyzerInterface.OnPacketArrival += OnPacketArrival;

            // Unsubscribe
            Unsubscribe = () =>
            {
                AnalyzerInterface.OnPacketArrival -= OnPacketArrival;
            };

            // Generate
            Generate(GeneratorInterface);

            // Wait for result
            Thread = new Thread(() => {
                if (!BlocingWaiting.WaitOne(Timeout))
                {
                    Stop(TestCaseStatus.Timeout);
                }
            });
            Thread.Start();
        }
        
        protected void Success()
        {
            Stop(TestCaseStatus.Success);
        }

        protected void Error()
        {
            Stop(TestCaseStatus.Error);
        }

        protected void Log(string Message)
        {
            OnLogMessage(Message);
        }

        public void Stop()
        {
            Stop(TestCaseStatus.Canceled);
        }

        public string Type => GetType().Name;

        public static Type GetType(string TestCaseName)
        {
            Type Type = Type.GetType(NS_PREFIX + TestCaseName);
            if (Type == null)
            {
                throw new Exception("TestCase not found.");
            }

            return Type;
        }
    }

    enum TestCaseStatus : int
    {
        Idle = 0,
        Running = 1,
        Success = 2,
        Error = 3,
        Timeout = 4,
        Canceled = 5
    }
}
