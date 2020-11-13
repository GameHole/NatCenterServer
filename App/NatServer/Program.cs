using System;
using System.Net;
using System.Threading;
using NatCore;
namespace NatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NatMgr.Init();
            Shares.GetShare<IEnv>().value = EnvEnum.Server;
            SimpleUdpServce servce = new SimpleUdpServce(25410);
            servce.Start();
            servce.Send(new ReqAsServer(), new IPEndPoint(IPAddress.Parse("47.105.195.93"), 12000));
            Thread.Sleep(-1);
        }
    }
}
