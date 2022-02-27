using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cs_Server
{
    class Program
    {
        readonly static List<Socket> clients = new List<Socket>();
        static void Main(string[] args)
        {
            using var listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            // 10.1.18.4
            var ip = IPAddress.Parse("10.1.18.4");
            var port = 45678;

            var ep = new IPEndPoint(ip, port);

            listener.Bind(ep);

            var backlog = 1;
            listener.Listen(backlog);


            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"Listening on {listener.LocalEndPoint}");
                var client = listener.Accept();
                clients.Add(client);
                Console.WriteLine($"{client.RemoteEndPoint} connected...");


                Task.Run(() =>
                {
                   var consoleColor = (ConsoleColor)((client.RemoteEndPoint as IPEndPoint).Port % 15);

                    var bytes = new byte[1024];
                    var len = 0;
                    var msg = "";

                    while (true)
                    {
                        len = client.Receive(bytes);
                        msg = Encoding.Default.GetString(bytes, 0, len);

                        if (msg == "exit")
                        {
                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                            break;
                        }

                        msg = $"{client.RemoteEndPoint} : {msg}";
                        
                        Console.ForegroundColor = consoleColor;
                        Console.WriteLine(msg);

                        foreach (var c in clients)
                        {
                            if(client!= c)
                            {
                                c.Send(Encoding.Default.GetBytes(msg));
                            }
                        }
                    }
                });
            }
        }
    }
}
