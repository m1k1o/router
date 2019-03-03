using WebSocketSharp;
using WebSocketSharp.Server;

namespace Router
{
    public class EchoReply : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "TEST"
                      ? "Successfull Test"
                      : "Reply";

            Send(msg);
        }
    }

    public class WebSockets
    {
        public static void Start()
        {
            var wssv = new WebSocketServer("ws://localhost:4649");
            wssv.AddWebSocketService<EchoReply>("/Echo");
            wssv.Start();
        }
    }
}
