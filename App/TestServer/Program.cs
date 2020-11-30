using NatCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TestServer
{
    struct TestMsg:IBytes
    {
        public int a;
    }
    struct TestMsgB : IBytes
    {
        public int a;
    }
    class TEstA : ADealer<TestMsg>
    {
        protected override void Deal(SimpleUdpServce socket, EndPoint remote, TestMsg value)
        {
            socket.Send(new TestMsgB() { a = value.a +1 }, remote);
            Console.WriteLine(value.a);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            NatMgr.Init();
            SimpleUdpServce servce = new SimpleUdpServce(27015);
            servce.Start();
            Thread.Sleep(-1);
        }

    }
}
