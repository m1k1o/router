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
        HttpListener httpListener = new HttpListener();

        public HTTP(String URL)
        {
            Console.WriteLine("Starting server...");
            httpListener.Prefixes.Add(URL);
            httpListener.Start();
            Console.WriteLine("Server started.");
        }

        private void request(HttpListenerContext context)
        {
            var Request = context.Request;
            string Data;
            using (var Reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            {
                Data = Reader.ReadToEnd();
            }

            if (context.Response.OutputStream.CanWrite)
            {
                string Response = response(context.Request.RawUrl, Data);
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

        private string response(string URL, string Data)
        {
            // Process Request
            String[] Args = URL.Split('/');

            if (Args.Length != 3)
            {
                return "<html><head><title>Router API</title></head>" +
                           "<body>Welcome to <strong>Router API</strong><br>Request URL: " + URL + "</em></body>" +
                       "</html>";
            }

            String ClassName = "Router.Controllers." + Args[1];
            String MethodName = Args[2];

            try
            {
                // Find a type you want to instantiate: you need to know the assembly it's in for it, we assume that all is is one assembly for simplicity
                // You should be careful, because ClassName should be full name, which means it should include all the namespaces, like "ConsoleApplication.MyClass"
                // Not just "MyClass"
                Type type = Assembly.GetExecutingAssembly().GetType(ClassName);
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

                // I've assumed that method we want to call is declared like this
                // public void MyMethod() { ... }
                // So we pass an instance to call it on and empty parameter list
                return (String) method.Invoke(instance, new object[0]);
            }
            catch (Exception e)
            {
                var obj = new JSONObject();
                obj.push("error", e.Message);
                return obj.ToString();
            }
        }

        public void listen()
        {
            while (true)
            {
                HttpListenerContext context = httpListener.GetContext();
                request(context);
            }
        }
        static void Main(string[] args)
        {
            var HTTP = new HTTP("http://localhost:5000/");
            HTTP.listen();
        }
    }
}
