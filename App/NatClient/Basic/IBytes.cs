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
    }
}
