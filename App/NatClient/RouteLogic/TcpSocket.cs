using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NatCore.RouteLogic
{
    class TcpSocket : ISocket
    {
        class Remote : IRemote
        {
            public Socket socket;
            public bool eq(IRemote other)
            {
                var mote = (other as Remote);
                return mote != null && mote.socket == socket;
            }
        }
        struct Info
        {
            public Remote remote;
            public byte[] array;
            public Exception exception;
        }
        Socket socket;
        ConcurrentQueue<Info> recvInfos = new ConcurrentQueue<Info>();
        ConcurrentQueue<Info> sendInfos = new ConcurrentQueue<Info>();
        Thread thread;
        public int Available => socket.Available;

        public void Dispose()
        {
            socket.Close();
            socket.Dispose();
        }
        void Accept()
        {
            while (true)
            {
                recv(socket.Accept());
            }
        }
        byte[] buffer = new byte[1024 * 4];
        async void recv(Socket s)
        {
            while (true)
            {
                int n = 0;
                try
                {
                    n = await Task<int>.Factory.StartNew(() =>
                    {
                        return s.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    }); 
                    if (n > 0)
                    {
                        byte[] data = new byte[n];
                        Array.Copy(buffer, 0, data, 0, n);
                        recvInfos.Enqueue(new Info { array = data, remote = new Remote { socket = s } });
                        if (thread != null)
                            thread.Interrupt();
                    }
                }
                catch (Exception se)
                {
                    recvInfos.Enqueue(new Info { remote = new Remote { socket = s }, exception=se });
                    if (thread != null)
                        thread.Interrupt();
                    break;
                }
            }
        }
        public void Init(int port = 0)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public int Recv(byte[] buffer, int offset, int size, out IRemote remote)
        {
            if(!recvInfos.TryDequeue(out Info info))
            {
                thread = Thread.CurrentThread;
                Thread.Sleep(Timeout.Infinite);
            }
            if (info.exception != null)
                throw info.exception;
            remote = info.remote;
            Array.Copy(info.array, 0, buffer, 0, info.array.Length);
            return info.array.Length;
        }

        public int Send(byte[] data, int offset, int size, IRemote remote)
        {
            sendInfos.Enqueue()
        }

        public void SetRemote(string ip, int port)
        {
            throw new NotImplementedException();
        }
    }
}
