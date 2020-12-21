using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NatCore.RouteLogic
{
    class ServerCenter
    {
        ISocket socket;
        Dictionary<IRemote, ServerRoute> routes = new Dictionary<IRemote, ServerRoute>();
        List<IRemote> catches = new List<IRemote>();
        public ServerCenter(int port)
        {
            socket.Init(port);
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
                int n = socket.Recv(buffer, 0, buffer.Length,out var end);

                int idx = 0;
                if (buffer.TryGet(ref idx, out ServerHeader header))
                {
                    if (header.header == Header.ReqConnect)
                    {
                        header.header = Header.ResConnect;
                        var send = header.ToByteArray();
                        socket.Send(send, 0, send.Length, end);
                        if (routes.ContainsKey(end)) continue;
                        var rout = new ServerRoute(end, header.port);
                        rout.Recv();
                        catches.Add(end);
                        routes.Add(end, rout);
                    }
                }
            }
        }
    }
}
