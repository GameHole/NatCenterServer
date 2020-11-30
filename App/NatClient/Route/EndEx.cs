using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NatCore
{
    static class EndEx
    {
        public static bool eq(this EndPoint a, EndPoint b)
        {
            var c = a as IPEndPoint;
            var d = b as IPEndPoint;
            return c.Address.Equals(d.Address) && c.Port == d.Port;
        }
    }
}
