using NatCore;
using System.Threading;
using System;
namespace App
{
    class CenterApp
    {
        static void Main(string[] args)
        {
            NatMgr.Init();
            SimpleUdpServce server = new SimpleUdpServce(12000);
            server.Start();
            Shares.GetShare<IEnv>().value = EnvEnum.Center;
            Console.WriteLine("center start");
            Thread.Sleep(-1);
        }
    }
}
