using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NatCore.RouteLogic
{
    class ServerLocal
    {
        ISocket socket;
        IRemote remoteEnd;
        IRemote localEnd;
        bool isRunTick;
        IRemote gameEnd;
        Dictionary<int, LocalClient> clients = new Dictionary<int, LocalClient>();
        List<int> catckes = new List<int>();
        public void Start(string remoteIP, int remotePort, string gameIP, int gamePort)
        {
            /*gameEnd = new IPEndPoint(IPAddress.Parse(gameIP), gamePort);
            remoteEnd = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);*/
            socket.Init();
            //localEnd = new IPEndPoint(IPAddress.Parse(gameIP), (socket.LocalEndPoint as IPEndPoint).Port);
            //Console.WriteLine("LocalServer::" + localEnd);
            Tick();
            Connect(gamePort);
            ClearUnuseClient();
            Recv();
        }
        async void Connect(int port)
        {
            byte[] connect = new ServerHeader() { header = Header.ReqConnect, port = port }.ToByteArray();
            while (!isRunTick)
            {
                socket.Send(connect, 0, connect.Length, remoteEnd);
                await Task.Delay(10);
            }
        }
        async void Tick()
        {
            byte[] tick = new RouteHeader() { header = Header.TICK }.ToByteArray();

            while (true)
            {
                try
                {
                    //if (!isRunTick)
                    //{
                    //    await Task.Delay(10);
                    //    continue;
                    //}
                    //Console.WriteLine("send tick");
                    socket.Send(tick, 0, tick.Length, remoteEnd);
                    await Task.Delay(100);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }
        async void ClearUnuseClient()
        {
            while (true)
            {
                try
                {
                    for (int i = catckes.Count - 1; i >= 0; i--)
                    {
                        var key = catckes[i];
                        if (clients.TryGetValue(key, out var cl))
                        {
                            cl.tickCount += 10;
                            if (cl.tickCount >= 20000)
                            {
                                clients.Remove(key);
                                catckes.Remove(key);
                                if (cl.isRun)
                                    cl.Dispose();
                            }
                        }
                        else
                        {
                            catckes.Remove(key);
                        }
                    }
                    await Task.Delay(10);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }
        void Recv()
        {
            byte[] buffer = new byte[8 * 1024];
            while (true)
            {
                try
                {
                    int n = socket.Recv(buffer, 0, buffer.Length, out var end);
                    //Console.WriteLine($"RouteServer:: Recv { Encoding.ASCII.GetString(buffer, 0, n)} remote = {end} ");
                    int idx = 0;
                    if (buffer.TryGet(ref idx, out Header header))
                    {
                        if (header.mask == Header.MASK)
                        {
                            if (remoteEnd.eq(end))
                            {
                                if (header.opcode == Header.ResConnect.opcode)
                                {
                                    Console.WriteLine("LocalServer connected");
                                    buffer.TryGet(ref idx, out int port);
                                    (remoteEnd as IPEndPoint).Port = port;
                                    isRunTick = true;
                                }
                                else if (header.opcode == Header.MSG.opcode)
                                {
                                    if (buffer.TryGet(ref idx, out int cid))
                                    {
                                        if (!clients.TryGetValue(cid, out var client))
                                        {
                                            client = new LocalClient(cid, gameEnd, localEnd);
                                            client.Start();
                                            clients.Add(cid, client);
                                            catckes.Add(cid);
                                        }
                                        if (client.isRun)
                                        {
                                            client.tickCount = 0;
                                            //Console.WriteLine($"RouteServer::size = {SizeOf<RouteHeader>.value} idx= {idx} cid = {cid}");
                                            buffer.TryGet(idx, out int mask);
                                            //Console.WriteLine($"RouteServer:: byffer = {buffer.ToHexStr()}");
                                            client.socket.SendTo(buffer, idx, n - idx, SocketFlags.None, gameEnd);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                socket.SendTo(buffer, 0, n, SocketFlags.None, remoteEnd);
                            }
                        }
                    }
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se);
                    if (se.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        continue;
                    }
                    if (se.SocketErrorCode == SocketError.Interrupted)
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
