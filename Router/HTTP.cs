using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Router.Helpers;

namespace Router
{
    class HTTP
    {
        private HttpListener httpListener = new HttpListener();

        private HTTP(String URL)
        {
            Console.WriteLine("Starting server...");
            httpListener.Prefixes.Add(URL);
            httpListener.Start();
            Console.WriteLine("Server started.");
        }

        private void Request(HttpListenerContext context)
        {
            var Request = context.Request;
            string Data;
            using (var Reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            {
                Data = Reader.ReadToEnd();
            }

            if (context.Response.OutputStream.CanWrite)
            {
                string Response = this.Response(context.Request.RawUrl, Data);
                byte[] ResponseBytes = Encoding.UTF8.GetBytes(Response);

                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.OutputStream.Write(ResponseBytes, 0, ResponseBytes.Length);
                context.Response.KeepAlive = false;
            }

            try
            {
                context.Response.Close();
            }
            catch { };
        }

        private string Response(string URL, string Data)
        {
            // Process Request
            String[] Args = URL.Split('/');

            if (Args.Length != 3)
            {
                return "<html><head><title>Router API</title></head>" +
                           "<body>Welcome to <strong>Router API</strong><br>Request URL: " + URL + "</em></body>" +
                       "</html>";
            }

            String ClassName = Args[1];
            String MethodName = Args[2];

            try
            {
                Type Type = Type.GetType("Router.Controllers." + ClassName);
                if (Type == null)
                {
                    throw new Exception("Class '" + ClassName + "' not found.");
                }

                MethodInfo MethodInfo = Type.GetMethod(MethodName);
                if (MethodInfo == null)
                {
                    throw new Exception("Method '" + MethodName + "' not found.");
                }

                object Object = MethodInfo.Invoke(null, new object[] { Data });
                return JSON.SerializeObject(Object);
            }
            catch (TargetInvocationException e)
            {
                return JSON.SerializeObject(JSON.Error(e.InnerException.Message));
            }
            catch (Exception e)
            {
                return JSON.SerializeObject(JSON.Error(e.InnerException.Message));
            }
        }

        private void Listen()
        {
            while (true)
            {
                HttpListenerContext context = httpListener.GetContext();
                Request(context);
            }
        }

        private static void Main(string[] args)
        {
            var Preload = Interfaces.Instance;

            var HTTP = new HTTP("http://localhost:5000/");
            HTTP.Listen();
        }
    }
}
