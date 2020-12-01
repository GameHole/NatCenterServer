using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NatCore;
namespace RouteCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            //ForTest(ref args);
            if (args.Length > 0)
            {
                if (int.TryParse(args[0],out var port))
                {
                    Console.WriteLine($"server start port = {port}");
                    Server server = new Server(port);
                    server.Start();
                }
            }
        }
        static void ForTest(ref string[] args)
        {
            args = new string[]
            {
                "12000"
            };
        }
    }
}
