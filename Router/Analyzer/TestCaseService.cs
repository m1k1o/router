using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Router.Analyzer
{
    class TestCaseService : WebSocketService
    {
        private Dictionary<WebSocket, TestCaseInstance> Instances = new Dictionary<WebSocket, TestCaseInstance>();

        private void Start(WebSocket Client, TestCase TestCase)
        {
            if (Instances.ContainsKey(Client))
            {
                Instances[Client].Unsubscribe();
            }
            else
            {
                Instances.Add(Client, new TestCaseInstance(Client));
            }

            try
            {
                Instances[Client].TestCase = TestCase;
                Instances[Client].Subscribe();
                TestCase.Start();
            }
            catch (Exception e)
            {
                HTTP.WebSockets.Send(Client, "test_case", new {
                    Error = true,
                    Running = false,
                    e.Message
                });
            }
        }

        private void Stop(WebSocket Client)
        {
            if (Instances.ContainsKey(Client))
            {
                Instances[Client].TestCase.Stop();
                Instances[Client].Unsubscribe();
            }
            else
            {
                HTTP.WebSockets.Send(Client, "test_case", new
                {
                    Running = false
                });
            }
        }

        public void OnConnect(WebSocket Client) { }

        public void OnDisconnect(WebSocket Client)
        {
            Stop(Client);
            Instances.Remove(Client);
        }

        public void OnMessage(WebSocket Client, string Message)
        {
            try
            {
                var Response = JSON.DeserializeAnonymousType(Message, new
                {
                    Key = (string)null,
                    Action = (string)null,
                    GeneratorInterface = (Interface)null,
                    AnalyzerInterface = (Interface)null,
                    TestCaseId = (int?)null,
                    TestCase = (TestCase)null
                });

                if (Response.Key == "test_case" && Response.Action == "start" && (Response.TestCase != null || Response.TestCaseId != null))
                {
                    TestCase TestCase;
                    if (Response.TestCaseId != null)
                    {
                        TestCase = TestCaseStorage.Get((int)Response.TestCaseId);
                    }
                    else
                    {
                        TestCase = Response.TestCase;
                    }

                    // Set Interfaces
                    TestCase.GeneratorInterface = Response.GeneratorInterface;
                    TestCase.AnalyzerInterface = Response.AnalyzerInterface;
                    
                    // Start
                    Start(Client, TestCase);
                    return;
                }

                if (Response.Key == "test_case" && Response.Action == "stop")
                {
                    Stop(Client);
                    return;
                }
            } catch { }
        }
    }

    class TestCaseInstance
    {
        public TestCase TestCase { get; set; }
        public WebSocket WebSocket { get; set; }

        public TestCaseInstance(WebSocket WebSocket)
        {
            this.WebSocket = WebSocket;
        }

        void OnStarted()
        {
            HTTP.WebSockets.Send(WebSocket, "test_case", new
            {
                Running = true,
                TestCase.Status,
                TimeOut = TestCase.Timeout.TotalSeconds
            });
        }

        void OnStopped()
        {
            HTTP.WebSockets.Send(WebSocket, "test_case", new
            {
                Running = false,
                TestCase.Status
            });
        }

        void OnLogMessage(string Log)
        {
            HTTP.WebSockets.Send(WebSocket, "test_case", new
            {
                Log
            });
        }

        public void Subscribe()
        {
            TestCase.OnStarted += OnStarted;
            TestCase.OnStopped += OnStopped;
            TestCase.OnLogMessage += OnLogMessage;
        }

        public void Unsubscribe()
        {
            TestCase.OnStarted -= OnStarted;
            TestCase.OnStopped -= OnStopped;
            TestCase.OnLogMessage -= OnLogMessage;
        }
    }
}
