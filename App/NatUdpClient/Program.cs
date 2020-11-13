using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NatCore;
namespace NatUdpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            NatMgr.Init();
            SimpleUdpServce simple = new SimpleUdpServce();
            Console.WriteLine($"client start ip = {simple.local}");
            Shares.GetShare<IEnv>().value = EnvEnum.Client;
            simple.Start();
            Console.WriteLine("nat start");
            var remote = new IPEndPoint(IPAddress.Parse("123.118.106.73"), 25410);
            //using (Socket natSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            //{
            //    natSocket.Bind(new IPEndPoint(IPAddress.Any, 25410));
            //    natSocket.SendTo(ResNat.SUCCESS.ToByteArray(), remote);
            //}
            simple.Send(new ReqNat() { address = remote.ValueAddress() }, new IPEndPoint(IPAddress.Parse("47.105.195.93"), 12000));
            //for (int i = 1; i < ushort.MaxValue; i++)
            //{
            //    simple.Send(ResNat.SUCCESS, new IPEndPoint(IPAddress.Parse("123.118.106.73"), i));
            //}
            Console.WriteLine("nat end");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
                simple.Send(ResNat.NO_SERVER_FOUND, new IPEndPoint(IPAddress.Parse("123.118.106.73"), 25410));
            }
            Thread.Sleep(-1);
        }
    }
}
