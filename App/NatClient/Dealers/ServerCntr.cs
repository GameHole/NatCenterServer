using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NatCore
{
    public interface IServerCntr:IShare
    {
        bool Contains(ValueAddress point);
        void Add(ValueAddress point,EndPoint endp);
        bool tryGet(ValueAddress point, out EndPoint endp);
    }
    [Bind(typeof(IServerCntr))]
    class ServerCntr : IServerCntr
    {
        Dictionary<ValueAddress,EndPoint> points = new Dictionary<ValueAddress, EndPoint>();
        public void Add(ValueAddress point, EndPoint endp)
        {
            points.Add(point, endp);
        }

        public bool Contains(ValueAddress point)
        {
            return points.ContainsKey(point);
        }

        public bool tryGet(ValueAddress point, out EndPoint endp)
        {
            return points.TryGetValue(point, out endp);
        }
    }
}
