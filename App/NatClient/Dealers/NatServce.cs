using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NatCore
{
    public struct ReqAsServer:IBytes { }
    public struct ResAsServer : IBytes
    {
        public ValueAddress address;
    }
    public struct ReqNat : IBytes 
    {
        public ValueAddress address;
    }
    //public struct NatPkg : IBytes { }
    public struct ResNat : IBytes
    {
        public static readonly ResNat SUCCESS = new ResNat() { result = 1 };
        public static readonly ResNat NO_SERVER_FOUND = new ResNat() { result = -1 };
        public int result;
        public bool Equal(ResNat other) => result == other.result;
    }
    public class ReqAsServerDealer : ADealer<ReqAsServer>
    {
        IServerCntr server;
        protected override void Deal(SimpleUdpServce socket,EndPoint remote, ReqAsServer value)
        {
            var v = remote.ValueAddress();
            if (!server.Contains(v))
            {
                server.Add(v, remote);
            }
            TestLog.Log($"server ip::{remote}");
            socket.Send(new ResAsServer() { address = v }, remote);
        }
    }
    public class ResAsServerDealer : ADealer<ResAsServer>
    {
        protected override void Deal(SimpleUdpServce socket, EndPoint remote, ResAsServer value)
        {
            TestLog.Log($"outer ip::{value.address.toEndPoint()}");
        }
    }
    public class ReqNatServerDealer : ADealer<ReqNat>
    {
        IServerCntr server;
        IEnv env;
        protected override void Deal(SimpleUdpServce socket, EndPoint remote, ReqNat value)
        {
            TestLog.Log($"ReqNatServerDealer recv");
            if (env.value == EnvEnum.Center)
            {
                if (!server.tryGet(value.address,out var endp))
                {
                    TestLog.Log($"nat ip::{endp} not found");
                    socket.Send(ResNat.NO_SERVER_FOUND, remote);
                   
                }
                else
                {
                    TestLog.Log($"server ip::{endp} client id::{remote}");
                    var remoteAddr = remote.ValueAddress();
                    //if (remoteAddr.ipaddress == value.address.ipaddress)
                    //    remoteAddr.ipaddress = ValueAddress.LocalAddr;
                    socket.Send(new ReqNat { address = remoteAddr }, endp);
                }
            }
            if (env.value == EnvEnum.Server)
            {
                var endp = value.address.toEndPoint();
                TestLog.Log($"nat start watting ip = {endp}");
                //using(Socket natSocket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp))
                //{
                //    natSocket.Bind(new IPEndPoint(IPAddress.Any, value.address.port));
                //    natSocket.SendTo(ResNat.SUCCESS.ToByteArray(), endp);
                //}
                //socket.remoteEnd = endp;
                socket.Send(ResNat.SUCCESS, endp);
                TestLog.Log($"nat end");
            }
        }
    }
    public class ResNatServerDealer : ADealer<ResNat>
    {
        protected override void Deal(SimpleUdpServce socket, EndPoint remote, ResNat value)
        {
            if (value.Equal(ResNat.NO_SERVER_FOUND))
                TestLog.Log("server  not found");
            else if (value.Equal(ResNat.SUCCESS))
                TestLog.Log("nat success");
        }
    }
}
