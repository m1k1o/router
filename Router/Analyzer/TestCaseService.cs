using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Router.Analyzer
{
    class TestCaseService : WebSocketService
    {
        private Dictionary<WebSocket, TestCase> Instances = new Dictionary<WebSocket, TestCase>();

        private void Start(WebSocket Client, TestCase TestCase)
        {
            TestCase.OnStarted += () =>
            {
                HTTP.WebSockets.Send(Client, "test_case", new
                {
                    Running = true,
                    TestCase.Status,
                    TimeOut = TestCase.Timeout.TotalSeconds
                });
            };

            TestCase.OnStopped += () => {
                HTTP.WebSockets.Send(Client, "test_case", new
                {
                    Running = false,
                    TestCase.Status
                });
                Stop(Client);
            };

            TestCase.OnLogMessage += (Message) => {
                HTTP.WebSockets.Send(Client, "test_case", new
                {
                    Log = Message
                });
            };

            try
            {
                TestCase.Start();
                Instances.Add(Client, TestCase);
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
                Instances[Client].Stop();
                Instances.Remove(Client);
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
}
