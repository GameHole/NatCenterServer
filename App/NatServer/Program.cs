using System;
using System.Net;
using System.Threading;
using NatCore;
using System.Net.Sockets;
namespace NatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //NatMgr.Init();
            //Shares.GetShare<IEnv>().value = EnvEnum.Server;
            //SimpleUdpServce servce = new SimpleUdpServce(25410);
            //servce.Start();
            //servce.Send(new ReqAsServer(), new IPEndPoint(IPAddress.Parse("47.105.195.93"), 12000));
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Parse("49.1.108.97"), 10000));
            Thread.Sleep(-1);
        }
    }
}
