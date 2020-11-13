using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NatCore
{
    public struct ValueAddress : IEquatable<ValueAddress>
    {
        public static readonly uint LocalAddr = IPAddress.Parse("127.0.0.1").toUintAddress();
        public uint ipaddress;
        public ushort port;

        public override bool Equals(object obj)
        {
            return obj is ValueAddress address && Equals(address);
        }

        public bool Equals(ValueAddress other)
        {
            return ipaddress == other.ipaddress &&
                   port == other.port;
        }

        public override int GetHashCode()
        {
            var hashCode = -385311406;
            hashCode = hashCode * -1521134295 + ipaddress.GetHashCode();
            hashCode = hashCode * -1521134295 + port.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ValueAddress a, ValueAddress b) => a.port == b.port && a.ipaddress == b.ipaddress;
        public static bool operator !=(ValueAddress a, ValueAddress b) => a.port != b.port || a.ipaddress != b.ipaddress;
    }
}
