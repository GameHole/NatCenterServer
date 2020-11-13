using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NatCore
{
    public interface IDealer
    {
        void Deal(SimpleUdpServce socket,EndPoint remote, byte[] buffer,int offset);
    }
    public abstract class ADealer<T> : IDealer where T : unmanaged,IBytes
    {
        public unsafe void Deal(SimpleUdpServce socket, EndPoint remote, byte[] buffer, int offset)
        {
            fixed(byte* c = buffer)
            {
                Deal(socket, remote, * (T*)(c + offset));
            }
        }
        protected abstract void Deal(SimpleUdpServce socket, EndPoint remote, T value);
    }
    public static class Msg
    {
        static int seed = 1;
        static Dictionary<Type, int> type2Id = new Dictionary<Type, int>();
        internal static Dictionary<int, IDealer> dealers = new Dictionary<int, IDealer>();

        public static void AutoBind()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var item in assembly.GetTypes())
                {
                    if (!typeof(IBytes).IsAssignableFrom(item)) continue;
                    if (item.IsAbstract || item.IsInterface || item.IsClass) continue;
                    int id = seed++;
                    type2Id.Add(item, id);
                    //Console.WriteLine($"msg::{item},id::{id}");
                }
            }
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var item in assembly.GetTypes())
                {
                    if (!typeof(IDealer).IsAssignableFrom(item)) continue;
                    if (item.IsAbstract || item.IsInterface) continue;
                    if (item.BaseType == null || !item.BaseType.IsGenericType) continue;
                    var msgType = item.BaseType.GenericTypeArguments[0];
                    if (type2Id.TryGetValue(msgType, out int id))
                    {
                        dealers.Add(id, Activator.CreateInstance(item) as IDealer);
                    }
                }
            }
        }
        public static bool TryGetId<T>(out int id)where T:unmanaged,IBytes
        {
            return type2Id.TryGetValue(typeof(T), out id);
        }
        public static bool TryGetDealer(int id,out IDealer dealer)
        {
            return dealers.TryGetValue(id, out dealer);
        }
    }

}
