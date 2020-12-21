using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NatCore.RouteLogic
{
    class ServerRoute : IDisposable
    {
        ISocket socket;
        IRemote serverPoint;
        int port;
        int headOffset = SizeOf<RouteHeader>.value;
        int seed = 1;
        Dictionary<int, IRemote> id2client;
        Dictionary<IRemote, int> client2id;
        internal int tickCount;
        internal bool isRun;
        //Dictionary<int,Action<byte[],int, EndPoint>>
        public ServerRoute(IRemote serverPoint, int port)
        {
            this.serverPoint = serverPoint;
            this.port = port;
            init();
            isRun = true;
        }
        void init()
        {
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Init(0);
            id2client = new Dictionary<int, IRemote>();
            client2id = new Dictionary<IRemote, int>();
            Console.WriteLine($"route start at port {port}");
        }
        public async void Recv()
        {
            if (isRun)
            {
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        if (socket.Available <= 0)
                        {
                            Thread.Sleep(1);
                            Recv();
                            return;
                        }
                        byte[] buffer = new byte[1024 * 16];
                        int n = socket.Recv(buffer, headOffset, buffer.Length - headOffset, out var endp);
                        Recv();
                        //Console.WriteLine($"RouteServer:: Recv { Encoding.ASCII.GetString(buffer, 0, n)} remote = {endp} ");
                        if (endp.eq(serverPoint))
                        {
                            int idx = headOffset;

                            if (buffer.TryGet(ref idx, out RouteHeader header))
                            {
                                //Console.WriteLine("header " + header.header.opcode);
                                if (Header.TICK == header.header)
                                {
                                    tickCount = 0;
                                    //Interlocked.Exchange(ref tickCount, 0);
                                }
                                else if (Header.MSG == header.header)
                                {
                                    tickCount = 0;
                                    // Interlocked.Exchange(ref tickCount, 0);
                                    if (id2client.TryGetValue(header.cid, out var point))
                                    {
                                        socket.Send(buffer, idx, n - headOffset, point);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!client2id.TryGetValue(endp, out var cid))
                            {
                                cid = seed++;
                                client2id.Add(endp, cid);
                                id2client.Add(cid, endp);
                            }
                            //Console.WriteLine($"RouteServer orgion:: {buffer.ToHexStr(0, n)}");
                            var header = new RouteHeader() { header = Header.MSG, cid = cid };
                            int id = 0;
                            buffer.TrySet(ref id, header);
                            //Console.WriteLine($"RouteServer after:: {buffer.ToHexStr(0, n)}");
                            socket.Send(buffer, 0, n + headOffset, serverPoint);
                        }
                    }).ConfigureAwait(false);
                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        //continue;
                    }
                    if (se.SocketErrorCode == SocketError.Interrupted)
                    {
                        isRun = false;
                        //continue;
                    }
                    Console.WriteLine(se);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("RouteServer:: Over");
                socket.Dispose();
            }
        }

        public void Dispose()
        {
            if (!isRun) return;
            isRun = false;
            socket.Dispose();
        }
    }
}
