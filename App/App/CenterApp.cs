using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using NatCore;
using System.Collections.Generic;
using System.Text;

namespace App
{
    class CenterApp
    {
        class TimeOutInfo
        {
            public int increase;
        }
        static readonly int TimeOut = 5000;
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static ConcurrentDictionary<EndPoint, TimeOutInfo> infos = new ConcurrentDictionary<EndPoint, TimeOutInfo>();
        static void Main(string[] args)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, 22548));

            Recv();

            DealTimeOut();
        }
        static async void Recv()
        {
            Console.WriteLine("recv start");
            EndPoint endp = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[1024 * 4];
            while (true)
            {
                await Task.Factory.StartNew(() =>
                {
                    int n = socket.ReceiveFrom(buffer, ref endp);
                    if (!infos.TryGetValue(endp, out var time))
                        infos.TryAdd(endp, new TimeOutInfo());
                    time.increase = 0;
                    var idx = 0;
                    if(buffer.TryGet(ref idx ,out ValueAddress address))
                    {
                        var ads = address.toEndPoint();
                        if (infos.ContainsKey(ads))
                        {
                            socket.SendTo(buffer, ads);
                        }
                    }
                });
            }
        }
        static void DealTimeOut()
        {
            List<EndPoint> removed = new List<EndPoint>();
            while (true)
            {
                removed.Clear();
                foreach (var item in infos.Keys)
                {
                    infos[item].increase += 10;
                    if (infos[item].increase >= TimeOut)
                    {
                        removed.Add(item);
                    }
                }
                foreach (var item in removed)
                {
                    infos.TryRemove(item, out var m);
                }
                Thread.Sleep(10);
            }
        }
    }
}
