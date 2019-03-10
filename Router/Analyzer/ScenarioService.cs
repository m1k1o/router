using Router.Helpers;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Router.Analyzer
{
    class ScenarioService : WebSocketService
    {
        private Dictionary<WebSocket, Scenario> Instances = new Dictionary<WebSocket, Scenario>();

        private void Start(WebSocket Client, Scenario Scenario)
        {
            /*
            TestCase.OnStarted += () =>
            {
                HTTP.WebSockets.Send(Client, "analyzer", new
                {
                    Running = true,
                    Status = TestCase.Status.ToString(),
                    TimeOut = TestCase.Timeout.TotalSeconds
                });
            };

            TestCase.OnStopped += () => {
                HTTP.WebSockets.Send(Client, "analyzer", new
                {
                    Running = false,
                    Status = TestCase.Status.ToString()
                });
                Stop(Client);
            };

            TestCase.OnLogMessage += (Message) => {
                HTTP.WebSockets.Send(Client, "analyzer", new
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
                HTTP.WebSockets.Send(Client, "analyzer", new {
                    Error = true,
                    Running = false,
                    e.Message
                });
            }
            */
        }

        private void Stop(WebSocket Client)
        {
            /*
            if (Instances.ContainsKey(Client))
            {
                Instances[Client].Stop();
                Instances.Remove(Client);
            }
            else
            {
                HTTP.WebSockets.Send(Client, "analyzer", new
                {
                    Running = false
                });
            }
            */
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
                    Scenario = (Scenario)null
                });

                if (Response.Key == "scenario" && Response.Action == "start" && Response.Scenario != null)
                {
                    // Set Interfaces
                    Response.Scenario.GeneratorInterface = Response.GeneratorInterface;
                    Response.Scenario.AnalyzerInterface = Response.AnalyzerInterface;

                    Start(Client, Response.Scenario);
                    return;
                }

                if (Response.Key == "scenario" && Response.Action == "stop")
                {
                    Stop(Client);
                    return;
                }
            } catch { }
        }
    }
}
