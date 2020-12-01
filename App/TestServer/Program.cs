using NatCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
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
        Dictionary<EndPoint, int> ds = new Dictionary<EndPoint, int>();
        int seed;
        protected override void Deal(SimpleUdpServce socket, EndPoint remote, TestMsg value)
        {
            if(!ds.TryGetValue(remote,out var id))
            {
                ds.Add(remote, ++seed);
            }

            socket.Send(new TestMsgB() { a = value.a +1 }, remote);
            Console.WriteLine($"recv id::{id} v::{value.a}");
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
