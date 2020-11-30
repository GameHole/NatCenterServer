using System;
using System.Collections.Generic;
using System.Text;

namespace NatCore
{
    public interface IBytes
    {
    }
    public static class BytesEx
    {
        public static byte[] Combine(this byte[] array, byte[] combined, int offset, int length)
        {
            byte[] newArr = new byte[array.Length + length];
            Array.Copy(array, 0, newArr, 0, array.Length);
            Array.Copy(combined, offset, newArr, array.Length, length);
            return newArr;
        }
        public static byte[] Combine(this byte[] array, byte[] combined)
        {
            return Combine(array, combined, 0, combined.Length);
        }
        public unsafe static byte[] Combine<T>(this byte[] array, T value) where T : unmanaged
        {
            byte[] newArr = new byte[array.Length + sizeof(T)];
            Array.Copy(array, 0, newArr, 0, array.Length);
            fixed (byte* na = newArr)
            {
                *(T*)(na + array.Length) = value;
            }
            return newArr;
        }
        public unsafe static byte[] ToByteArray<T>(this T value) where T : unmanaged
        {
            byte[] newArr = new byte[sizeof(T)];
            fixed (byte* na = newArr)
            {
                *(T*)na = value;
            }
            return newArr;
        }
        public static byte[] ToByteArray(this string value)
        {
            byte[] body = Encoding.Default.GetBytes(value);
            return body.Length.ToByteArray().Combine(body);
        }
        public static unsafe T Get<T>(this byte[] value,ref int offset)where T:unmanaged
        {
            fixed (byte* m = value)
            {
                var ret = *(T*)(m + offset);
                offset += sizeof(T);
                return ret;
            }
        }
        public static unsafe bool TryGet<T>(this byte[] value,ref int offset,out T v)where T : unmanaged
        {
            int size = sizeof(T);
            v = default;
            if (value.Length - offset < size)
                return false;
            fixed (byte* m = value)
            {
                v = *(T*)(m + offset);
                offset += size;
                return true;
            }
        }
        public static unsafe bool TryGet<T>(this byte[] value, int offset, out T v) where T : unmanaged
        {
            int size = sizeof(T);
            v = default;
            if (value.Length - offset < size)
                return false;
            fixed (byte* m = value)
            {
                v = *(T*)(m + offset);
                return true;
            }
        }
        public static unsafe bool TrySet<T>(this byte[] value, ref int offset,T v) where T : unmanaged
        {
            int size = sizeof(T);
            if (value.Length - offset < size)
                return false;
            fixed (byte* m = value)
            {
                *(T*)(m + offset) = v;
                offset += size;
                return true;
            }
        }
        public static string ToHexStr(this byte[] value)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                string a = value[i].ToString("X");
                if (value[i] < 0x10)
                    a = "0" + a;
                builder.Append(a);
                builder.Append(' ');
            }
            return builder.ToString();
        }
        public static string ToHexStr(this byte[] value,int offset,int size)
        {
            StringBuilder builder = new StringBuilder();
            int n = size + offset;
            for (int i = offset; i < n; i++)
            {
                if (i >= value.Length)
                    break;
                string a = value[i].ToString("X");
                if (value[i] < 0x10)
                    a = "0" + a;
                builder.Append(a);
                builder.Append(' ');
            }
            return builder.ToString();
        }
    }
}
