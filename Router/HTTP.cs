using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using PacketDotNet;
using Router.Helpers;
using Router.Protocols;
using Router.Protocols.DHCPOptions;

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

                object response = MethodInfo.Invoke(null, new object[] { Data });
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
           // var Preload = Interfaces.Instance;
           /*
            var MyClass = DHCPOption.Factory(1, 10, new Byte[] { 1, 2, 3, 4, 5 });
            Console.Write(MyClass.GetType());
            Console.ReadKey();
            */
            /*
            var HTTP = new HTTP("http://localhost:5000/");
            HTTP.Listen();
            */
            var Interface = Interfaces.Instance.GetInterfaceById("2");
            Interface.SetIP(IPAddress.Parse("192.168.1.5"), IPSubnetMask.Parse("255.255.255.0"));
            Interface.Start();

            Console.WriteLine("Started");

            var Options = new DHCPOptionCollection
            {
                new DHCPPadOption(5),
                new DHCPSubnetMaskOption(IPAddress.Parse("192.168.2.5")),
                new DHCPRouterOption(new List<IPAddress> {
                    IPAddress.Parse("8.8.8.8"),
                    IPAddress.Parse("10.10.2.5"),
                    IPAddress.Parse("192.168.1.0")
                }),
                new DHCPDomainNameServerOption(new List<IPAddress> {
                    IPAddress.Parse("8.9.8.8"),
                    IPAddress.Parse("10.1.2.5"),
                    IPAddress.Parse("192.4.1.0")
                }),
                new DHCPRequestedIPAddressOption(IPAddress.Parse("9.8.1.0"))
            };

            var dhcpPacket = new DHCPPacket
            {
                OperationCode = DHCPOperatonCode.BootRequest,
                HardwareType = LinkLayers.Ethernet,
                HardwareAddressLength = 6,
                TransactionID = 0x3903F326,
                ClientMACAddress = Interface.PhysicalAddress,
                IsDHCP = true,
                Options = Options
            };

            Console.WriteLine(Encoding.Default.GetString(dhcpPacket.Options.Bytes));

            var udpPacket = new UdpPacket(68, 67)
            {
                PayloadData = dhcpPacket.Bytes
            };

            var ipPacket = new IPv4Packet(IPAddress.Parse("0.0.0.0"), IPAddress.Parse("255.255.255.255"))
            {
                PayloadPacket = udpPacket
            };
            ipPacket.Checksum = ipPacket.CalculateIPChecksum();

            var ethernetPacket = new EthernetPacket(Interface.PhysicalAddress, PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"), EthernetPacketType.IpV4)
            {
                PayloadPacket = ipPacket
            };

            Interface.SendPacket(ethernetPacket.Bytes);
            Console.WriteLine("Sent");
        }
    }
}
