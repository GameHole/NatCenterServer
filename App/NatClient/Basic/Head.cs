using System;
using System.Collections.Generic;
using System.Text;

namespace NatCore
{
    public unsafe static class SizeOf<T>where T : unmanaged
    {
        public static int value => sizeof(T);
    }
    public unsafe struct Head
    {
        public static int size => sizeof(Head);
        public static readonly uint defauleMark = 0xFAECAD00;
        public uint mark;
        public int opcode;
        //public unsafe byte[] ToBytes()
        //{
        //    byte[] array = new byte[sizeof(Head)];
        //    fixed (byte* a = array)
        //    {
        //        *(uint*)a = mark;
        //        *(a + sizeof(uint)) = code;
        //    }
        //    return array;
        //}
    }
}
