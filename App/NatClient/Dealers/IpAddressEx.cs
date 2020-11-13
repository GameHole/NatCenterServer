using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NatCore
{
    public static class IpAddressEx
    {
        public static unsafe uint toUintAddress(this IPAddress address)
        {
            fixed (byte* m = address.GetAddressBytes())
            {
                return *(uint*)m;
            }
        }
        public static unsafe IPAddress GetAddress(this ValueAddress address)
        {
            return new IPAddress(address.ipaddress.ToByteArray());
        }
        public static unsafe EndPoint toEndPoint(this ValueAddress address)
        {
            byte[] addarr = new byte[sizeof(uint)];
            fixed (byte* m = addarr)
                *(uint*)m = address.ipaddress;
            IPAddress ip = new IPAddress(addarr);
            return new IPEndPoint(ip, address.port);
        }
        public static unsafe ValueAddress ValueAddress(this EndPoint endPoint)
        {
            ValueAddress address = new ValueAddress();
            var end = (endPoint as IPEndPoint);
            address.ipaddress = end.Address.toUintAddress();
            address.port = (ushort)end.Port;
            return address;
        }
    }
}
