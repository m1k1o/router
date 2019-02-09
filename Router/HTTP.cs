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
                // Find a type you want to instantiate: you need to know the assembly it's in for it, we assume that all is is one assembly for simplicity
                // You should be careful, because ClassName should be full name, which means it should include all the namespaces, like "ConsoleApplication.MyClass"
                // Not just "MyClass"
                Type type = Assembly.GetExecutingAssembly().GetType("Router.Controllers." + ClassName);
                if (type == null)
                {
                    throw new Exception("Class '"+ ClassName+"' not found.");
                }

                // Get MethodInfo, reflection class that is responsible for storing all relevant information about one method that type defines
                MethodInfo method = type.GetMethod(MethodName);
                if (method == null)
                {
                    throw new Exception("Method '" + MethodName + "' not found.");
                }

                // Create an instance of the type
                object instance = Activator.CreateInstance(type);

                // So we pass an instance to call it on and parameter list
                object response = method.Invoke(instance, new object[] { Data });
                return new JSON(response).ToString();
            }
            catch (TargetInvocationException e)
            {
                return new JSONError(e.InnerException.Message).ToString();
            }
            catch (Exception e)
            {
                return new JSONError(e.Message).ToString();
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
