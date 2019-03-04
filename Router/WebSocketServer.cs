using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Router
{
    public class Util
    {
        public static string GetString(byte[] bytes, int count)
        {
            return System.Text.Encoding.ASCII.GetString(bytes, 0, count);
        }

        public static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }
    }

    public class WebSocketDictionary : IDictionary<String, WebSocket>
    {
        protected Dictionary<String, WebSocket> _innerDictionary;
        protected bool _isReadOnly;

        public WebSocketDictionary()
        {
            _innerDictionary = new Dictionary<string, WebSocket>();
        }

        public async Task SendBroadcastMessage(String message)
        {
            foreach (WebSocket webSocket in _innerDictionary.Values)
                await SendMessage(webSocket, message);
        }

        public async Task SendMessage(string key, string message)
        {
            await SendMessage(_innerDictionary[key], message);
        }

        public async Task SendMessage(WebSocket webSocket, string message)
        {
            byte[] buffer = Util.GetBytes(message);

            await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
        }



        #region Default Methods from ICollection Interface

        public void Add(string key, WebSocket value)
        {
            _innerDictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _innerDictionary.Keys; }
        }

        public bool Remove(string key)
        {
            return _innerDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out WebSocket value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        public ICollection<WebSocket> Values
        {
            get { return _innerDictionary.Values; }
        }

        public WebSocket this[string key]
        {
            get
            {
                return _innerDictionary[key];
            }
            set
            {
                _innerDictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<string, WebSocket> item)
        {
            _innerDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, WebSocket> item)
        {
            return _innerDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, WebSocket>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        public bool Remove(KeyValuePair<string, WebSocket> item)
        {
            if (_innerDictionary[item.Key].Equals(item.Value))
                return _innerDictionary.Remove(item.Key);

            return false;
        }

        public IEnumerator<KeyValuePair<string, WebSocket>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class WebSocketServer
    {
        public static Action<String, String> onMessage;
        public static Action<String> onConnect;
        public static Action<String, Exception> onError;

        private static WebSocketDictionary webSocketDictionary = new WebSocketDictionary();
        
        public async void Start(string httpListenerPrefix)
        {
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(httpListenerPrefix);
            httpListener.Start();
            Console.WriteLine("Listening...");

            webSocketDictionary = new WebSocketDictionary();
            while (true)
            {
                HttpListenerContext httpListenerContext = await httpListener.GetContextAsync();
                if (httpListenerContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(httpListenerContext);
                }
                else
                {
                    httpListenerContext.Response.StatusCode = 400;
                    httpListenerContext.Response.Close();
                }
            }
        }

        public static void SendMessage(string key, string message)
        {
            webSocketDictionary.SendMessage(key, message);
        }

        public static void SendBroadcastMessage(string message)
        {
            webSocketDictionary.SendBroadcastMessage(message);
        }

        public static WebSocketState WebSocketStatus(string key)
        {
            return webSocketDictionary[key].State;
        }

        public async static void ProcessRequest(HttpListenerContext httpListenerContext)
        {
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await httpListenerContext.AcceptWebSocketAsync(subProtocol: null);
                string ipAddress = httpListenerContext.Request.RemoteEndPoint.Address.ToString();
                Console.WriteLine(String.Format("Connected: IPAddress {0}", ipAddress));

                if (onConnect != null && httpListenerContext.Request.Headers.AllKeys.Contains("Sec-WebSocket-Key"))
                    onConnect(httpListenerContext.Request.Headers["Sec-WebSocket-Key"]);
            }
            catch (Exception ex)
            {
                httpListenerContext.Response.StatusCode = 500;
                httpListenerContext.Response.Close();

                if (onError != null)
                    onError(String.Empty, ex);

                Console.WriteLine(String.Format("Exception: {0}", ex));
                return;
            }

            WebSocket webSocket = webSocketContext.WebSocket;
            String requestKey = httpListenerContext.Request.Headers["Sec-WebSocket-Key"];
            webSocketDictionary.Add(requestKey, webSocket);

            try
            {
                byte[] receiveBuffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), System.Threading.CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", System.Threading.CancellationToken.None);
                        webSocketDictionary.Remove(requestKey);
                    }
                    else
                    {
                        string message = Util.GetString(receiveBuffer, receiveResult.Count);
                        if (onMessage != null)
                            onMessage(requestKey, message);

                        await webSocketDictionary.SendBroadcastMessage(message);
                        receiveBuffer = new byte[1024];
                    }
                }
            }
            catch (Exception ex)
            {
                if (onError != null)
                    onError(requestKey, ex);

                Console.WriteLine(String.Format("Exception: {0}", ex));
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
            }
        }
    }
}