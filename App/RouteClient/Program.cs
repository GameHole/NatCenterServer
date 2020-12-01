using System;
using NatCore;
namespace RouteClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //ForTest(ref args);
            if (args.Length > 3)
            {
                LocalServer server = new LocalServer();
                server.Start(args[0], int.Parse(args[1]), args[2], int.Parse(args[3]));
            }
        }
        static void ForTest(ref string[] args)
        {
            args = new string[]
            {
                "47.105.195.93",
                "12000",
                "127.0.0.1",
                "27015"
            };
        }
    }
}
