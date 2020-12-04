using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NatCore;
using System.Threading.Tasks;
using System.Threading;

namespace NatCore
{
    struct ServerHeader
    {
        public Header header;
        public int port;
    }
    public class Server
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Dictionary<EndPoint,RouteServer> routes = new Dictionary<EndPoint, RouteServer>();
        List<EndPoint> catches = new List<EndPoint>();
        public Server(int port)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any,port));
        }
        async void Tick()
        {
            while (true)
            {
                try
                {
                    for (int i = catches.Count - 1; i >= 0; i--)
                    {
                        var key = catches[i];
                        if (routes.TryGetValue(key, out var route))
                        {
                            int tick = Interlocked.Add(ref route.tickCount, 10);
                            //Console.WriteLine($"Server:: tick = {tick}");

                            if (tick >= 1000)
                            {
                                routes.Remove(key);
                                catches.Remove(key);
                                Console.WriteLine($"Server::Client Lost EndPoint = {key}");
                                route.Dispose();
                            }
                        }
                        else
                        {
                            catches.Remove(key);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                await Task.Delay(10);
            }
        }
        public void Start()
        {
            Tick();
            byte[] buffer = new byte[1024];
            while (true)
            {
                EndPoint end = new IPEndPoint(IPAddress.Any, 0);
                int n = socket.ReceiveFrom(buffer,ref end);
               
                int idx = 0;
                if(buffer.TryGet(ref idx,out ServerHeader header))
                {
                    if (header.header == Header.ReqConnect)
                    {
                        header.header = Header.ResConnect;
                        socket.SendTo(header.ToByteArray(), end);
                        if (routes.ContainsKey(end)) continue;
                        var rout = new RouteServer(end, header.port);
                        rout.Recv();
                        catches.Add(end);
                        routes.Add(end, rout);
                    }
                }
            }
        }
    }
}
