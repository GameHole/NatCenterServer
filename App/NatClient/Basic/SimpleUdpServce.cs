using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NatCore
{
    public class SimpleUdpServce
    {
        Socket socket;
        public EndPoint local;
        public EndPoint remoteEnd;
        bool isRun;
        public SimpleUdpServce(int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            local = socket.LocalEndPoint;
            
        }
        public SimpleUdpServce() : this(0) { }
        public async void Start()
        {
            try
            {
                isRun = true;
                byte[] buffer = new byte[1024];
                await Task.Factory.StartNew(() =>
                {
                    while (isRun)
                    {
                        int idx = 0;
                        int recv = 0;
                        if (remoteEnd == null)
                            remoteEnd = new IPEndPoint(IPAddress.Any, 0);
                        try
                        {
                            recv = socket.ReceiveFrom(buffer, ref remoteEnd);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        finally
                        {
                            TestLog.Log($"ddd:{remoteEnd}");
                        }
                        //Console.WriteLine("SimpleUdpServce recv");
                        if (recv < Head.size) continue;
                        var head = buffer.Get<Head>(ref idx);
                        //Console.WriteLine($"SimpleUdpServce recv size ok mask ={ head.mark} vailed = {Head.defauleMark}");
                        if (head.mark != Head.defauleMark) continue;
                        //Console.WriteLine("SimpleUdpServce recv mask ok");
                        if (Msg.TryGetDealer(head.opcode, out IDealer dealer))
                        {
                            dealer.Deal(this, remoteEnd, buffer, idx);
                        }
                        else
                        {
                            TestLog.Log($"warnning:: msg id = {head.opcode} not found dealer");
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception e)
            {
                TestLog.Log($"error:: {e}");
            }
        }
        public void Send<T>(T msg,EndPoint point)where T : unmanaged, IBytes
        {
            if (!isRun) return;
            if(!Msg.TryGetId<T>(out int id))
            {
                Console.WriteLine($"warnning:: msg type= {typeof(T)} not registed");
                return;
            }
            var head = new Head();
            head.mark = Head.defauleMark;
            head.opcode = id;
            socket.SendTo(head.ToByteArray().Combine(msg), point);
        }
        public void Dispose()
        {
            isRun = false;
            socket.Shutdown(SocketShutdown.Both);
            socket.Dispose();
        }
    }
}
