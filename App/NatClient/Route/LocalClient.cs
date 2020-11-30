using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using NatCore;

namespace NatCore
{
    class LocalClient
    {
        internal Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        EndPoint serverEnd;
        EndPoint gameEnd;
        internal bool isRun;
        internal int tickCount;
        int id;
        public LocalClient(int id,EndPoint game,EndPoint server)
        {
            this.id = id;
            this.gameEnd = game;
            this.serverEnd = server;
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        }
        public async void Start()
        {
            isRun = true;
            byte[] fromBuffer = new byte[1024 * 8];
            while (isRun)
            {
                try
                {
                    await Task.Factory.StartNew(() =>
                    {
                        int size = SizeOf<RouteHeader>.value;
                        int n = socket.ReceiveFrom(fromBuffer, size, fromBuffer.Length- size, SocketFlags.None, ref gameEnd);
                        tickCount = 0;
                        int idx = 0;
                        fromBuffer.TrySet(ref idx, new RouteHeader { header = Header.MSG, cid = id });
                        socket.SendTo(fromBuffer, 0, n + size, SocketFlags.None, serverEnd);
                    });
                }
                catch (SocketException se)
                {
                    if(se.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        isRun = false;
                    }
                    if (se.SocketErrorCode == SocketError.Interrupted)
                    {
                        isRun = false;
                    }
                    Console.WriteLine(se);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            socket.Dispose();
        }
        public void Dispose()
        {
            isRun = false;
            socket.Shutdown(SocketShutdown.Both);
            socket.Dispose();
        }
    }
}
