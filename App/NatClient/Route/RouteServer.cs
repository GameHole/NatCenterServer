using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NatCore;
using System.Threading;
using System.Threading.Tasks;

namespace NatCore
{
    //static class Operation
    //{
    //    //public static readonly int MASK = 0x74141908;
    //    //public const int Tick = 10;
    //    //public const int MSG = 11;
    //    //public const int ReqConnect = 2;
    //    //public const int ResConnect = 3;
    //}

    internal struct Header : IEquatable<Header>
    {
        public static readonly int MASK = 0x74141908;
        public static readonly Header ReqConnect = new Header { mask = MASK, opcode = 2 };
        public static readonly Header ResConnect = new Header { mask = MASK, opcode = 3 };
        public static readonly Header MSG = new Header { mask = MASK, opcode = 11 };
        public static readonly Header TICK = new Header { mask = MASK, opcode = 10 };
        public int mask;
        public int opcode;

        public override bool Equals(object obj)
        {
            return obj is Header header && Equals(header);
        }

        public bool Equals(Header other)
        {
            return mask == other.mask &&
                   opcode == other.opcode;
        }

        public override int GetHashCode()
        {
            var hashCode = 249528824;
            hashCode = hashCode * -1521134295 + mask.GetHashCode();
            hashCode = hashCode * -1521134295 + opcode.GetHashCode();
            return hashCode;
        }

        public static bool operator==(Header a,Header b)
        {
            return a.mask == b.mask && a.opcode == b.opcode;
        }
        public static bool operator !=(Header a, Header b)
        {
            return a.mask != b.mask || a.opcode != b.opcode;
        }
    }
    struct RouteHeader
    {
        public Header header;
        public int cid;
    }
    public class RouteServer:IDisposable
    {
        Socket socket;
        EndPoint serverPoint;
        int port;
        int headOffset = SizeOf<RouteHeader>.value;
        int seed = 1;
        Dictionary<int, EndPoint> id2client;
        Dictionary<EndPoint, int> client2id;
        internal int tickCount;
        internal bool isRun;
        //Dictionary<int,Action<byte[],int, EndPoint>>
        public RouteServer(EndPoint serverPoint, int port)
        {
            this.serverPoint = serverPoint;
            this.port = port;
            init();
            isRun = true;
        }
        void init()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            id2client = new Dictionary<int, EndPoint>();
            client2id = new Dictionary<EndPoint, int>();
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
                        if(socket.Available<=0)
                        {
                            Thread.Sleep(1);
                            Recv();
                            return;
                        }
                        byte[] buffer = new byte[1024 * 4];
                        EndPoint endp = new IPEndPoint(IPAddress.Any, 0);
                        int n = socket.ReceiveFrom(buffer, headOffset, buffer.Length - headOffset, SocketFlags.None, ref endp);
                        Recv();
                        //Console.WriteLine($"RouteServer:: Recv { Encoding.ASCII.GetString(buffer, 0, n)} remote = {endp} ");
                        if (endp.eq(serverPoint))
                        {
                            int idx = headOffset;
                            if (buffer.TryGet(ref idx, out RouteHeader header))
                            {
                                if (Header.TICK == header.header)
                                {
                                    tickCount = 0;
                                }
                                else if (Header.MSG == header.header)
                                {
                                    if (id2client.TryGetValue(header.cid, out var point))
                                    {
                                        socket.SendTo(buffer, idx, n - headOffset, SocketFlags.None, point);
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
                            //Console.WriteLine($"RouteServer orgion:: {buffer.ToHexStr()}");
                            var header = new RouteHeader() { header = Header.MSG, cid = cid };
                            int id = 0;
                            buffer.TrySet(ref id, header);
                            //Console.WriteLine($"RouteServer after:: {buffer.ToHexStr()}");
                            socket.SendTo(buffer, 0, n + headOffset, SocketFlags.None, serverPoint);
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
                socket.Close();
                socket.Dispose();
            }
        }

        public void Dispose()
        {
            if (!isRun) return;
            isRun = false;
            socket.Close();
            socket.Dispose();
        }
    }
}
