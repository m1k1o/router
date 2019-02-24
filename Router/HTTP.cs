using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using PacketDotNet;
using Router.Helpers;
using Router.Packets;

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

            String NamespaceName = Args[1];
            String ClassName = Args[2];

            try
            {
                Type Type = Type.GetType("Router.Controllers." + NamespaceName + "." + ClassName);
                if (Type == null)
                {
                    throw new Exception("Endpoint'" + NamespaceName + "." + ClassName + "' not found.");
                }

                ConstructorInfo Constructor = Type.GetConstructor(Type.EmptyTypes);
                var Controller = (Controllers.Controller)Constructor.Invoke(Type.EmptyTypes);

                // Populate
                if (!string.IsNullOrEmpty(Data))
                {
                    JSON.PopulateObject(Data, Controller);
                }

                // Execute
                if (Controller is Controllers.Executable)
                {
                    ((Controllers.Executable)Controller).Execute();
                }

                // Export
                return JSON.SerializeObject(Controller.Export());
            }
            catch (Exception e)
            {
                return JSON.SerializeObject(JSON.Error(e.Message));
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

            var Interface = Interfaces.Instance.GetInterfaceById("1");
            Interface.SetIP(IPAddress.Parse("192.168.1.5"), IPSubnetMask.Parse("255.255.0.0"));
            Interface.Start();

            //var HTTP = new HTTP("http://localhost:5000/");
            //HTTP.Listen();

            var eth = new Packets.Ethernet
            {
                SourceHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                DestinationHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                EthernetPacketType = EthernetPacketType.IpV4
            };
            var ip = new Packets.IP
            {
                SourceAddress = IPAddress.Parse("192.168.1.1"),
                DestinationAddress = IPAddress.Parse("192.168.1.1"),
                IPProtocolType = IPProtocolType.UDP
            };
            var udp = new Packets.UDP
            {
                SourcePort = 50,
                DestinationPort = 50
            };

            ip.PayloadData(udp.Export());
            eth.PayloadData(ip.Export());
            var Response = eth.Export();

            /*
            var Packets = new List<PacketsImportExport>()
            {
                new Packets.Ethernet
                {
                    SourceHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                    DestinationHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                    EthernetPacketType = EthernetPacketType.IpV4
                },
                new Packets.IP
                {
                    SourceAddress = IPAddress.Parse("192.168.1.1"),
                    DestinationAddress = IPAddress.Parse("192.168.1.1"),
                    IPProtocolType = IPProtocolType.UDP
                },
                new Packets.UDP
                {
                    SourcePort = 50,
                    DestinationPort = 50
                },
                new Packets.ARP
                {
                    Operation = ARPOperation.Request,
                    SenderHardwareAddress = PhysicalAddress.Parse("00-00-00-00-00-00"),
                    SenderProtocolAddress = IPAddress.Parse("0"),
                    TargetHardwareAddress = PhysicalAddress.Parse("AA-BB-CC-DD-EE-FF"),
                    TargetProtocolAddress = IPAddress.Parse("0")
                }
            };

            var Response = Generator.Parse(Packets);
            */
            Interface.SendPacket(Response);

            /*
            var arp = new Packets.ARP
            {
                SourceHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),
                DestinationHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"),

                Operation = ARPOperation.Request,
                SenderHardwareAddress = PhysicalAddress.Parse("00-00-00-00-00-00"),
                SenderProtocolAddress = IPAddress.Parse("0"),
                TargetHardwareAddress = PhysicalAddress.Parse("AA-BB-CC-DD-EE-FF"),
                TargetProtocolAddress = IPAddress.Parse("0")
            };

            var ByteArray = arp.Export();

            var arp2 = new Packets.ARP();
            arp2.ImportAll(ByteArray);


            Console.WriteLine(arp.TargetHardwareAddress);

            Console.ReadKey();
            */
        }
    }
}
