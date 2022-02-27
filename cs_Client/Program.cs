using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace cs_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);


            var ip = IPAddress.Parse("127.0.0.1");
            var port = 45678;
            var endPoint = new IPEndPoint(ip, port);

            try
            {
                client.Connect(endPoint);

                if (client.Connected)
                {
                    Console.WriteLine("Connected to the server...");

                    Task.Run(() =>
                    {
                        var bytes = new byte[1024];
                        var len = 0;
                        var msg = "";

                        while (true)
                        {
                            len = client.Receive(bytes);
                            msg = Encoding.Default.GetString(bytes, 0, len);
                            Console.WriteLine(msg);
                        }

                    });


                    while (true)
                    {
                        var msg = Console.ReadLine();
                        var bytes = Encoding.Default.GetBytes(msg);
                        client.Send(bytes);
                    }
                }
                else
                {
                    Console.WriteLine("Can not connect to the server...");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("EX: Can not connect to the server...");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
