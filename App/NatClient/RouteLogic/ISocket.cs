using System;
using System.Collections.Generic;
using System.Text;

namespace NatCore.RouteLogic
{
    public interface ISocket:IDisposable
    {
        int Available{ get; }
        void Init(int port = 0);
        void SetRemote(string ip,int port);
        int Send(byte[] data, int offset, int size, IRemote remote);
        int Recv(byte[] buffer, int offset, int size,out IRemote remote);
    }
}
