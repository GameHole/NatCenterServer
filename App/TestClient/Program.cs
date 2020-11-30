using NatCore;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    struct TestMsg : IBytes
    {
        public int a;
    }
    struct TestMsgB : IBytes
    {
        public int a;
    }
    class TEstB : ADealer<TestMsgB>
    {
        protected override async void Deal(SimpleUdpServce socket, EndPoint remote, TestMsgB value)
        {
            await Task.Delay(100);
            socket.Send(new TestMsg() { a = value.a + 1 }, remote);
            Console.WriteLine(value.a);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            NatMgr.Init();
            SimpleUdpServce servce = new SimpleUdpServce();
            servce.Start();
            servce.Send(new TestMsg() { a = 1 }, new IPEndPoint(IPAddress.Parse("47.105.195.93"), 27015));
            Thread.Sleep(-1);
        }
    }
}
