using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NatCore
{
    public interface IShare { }
    public interface IAwake 
    {
        void Awake();
    }
    public class BindAttribute : Attribute
    {
        public Type interType;
        public BindAttribute(Type type)
        {
            interType = type;
        }
    }
    public static class Shares
    {
        internal static Dictionary<Type, IShare> shares = new Dictionary<Type, IShare>();
        public static void AutoBind()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var item in assembly.GetTypes())
                {
                    if (!typeof(IShare).IsAssignableFrom(item)) continue;
                    if (item.IsAbstract || item.IsInterface) continue;
                    var attr = item.GetCustomAttribute<BindAttribute>();
                    if (attr == null) continue;
                    var inter = attr.interType;
                    if (!typeof(IShare).IsAssignableFrom(inter) || !inter.IsInterface) continue;
                    shares.Add(inter, Activator.CreateInstance(item) as IShare);
                }
            }
            foreach (var item in shares.Values)
            {
                (item as IAwake)?.Awake();
            }
        }
        public static T GetShare<T>() where T : IShare
        {
            shares.TryGetValue(typeof(T), out var share);
            return (T)share;
        }
        public static void Inject(object o)
        {
            foreach (var item in o.GetType().GetFields(BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic))
            {
                if(shares.TryGetValue(item.FieldType,out var share))
                {
                    item.SetValue(o, share);
                }
            }
        }
    }
    public static class NatMgr
    {
        public static void Init()
        {
            Msg.AutoBind();
            Shares.AutoBind();
            foreach (var item in Msg.dealers.Values)
            {
                Shares.Inject(item);
            }
        }
    }
}
