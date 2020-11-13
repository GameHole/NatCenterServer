using System;
using System.Collections.Generic;
using System.Text;

namespace NatCore
{
    public enum EnvEnum
    {
        Center,Server,Client
    }
    public interface IEnv:IShare
    {
        EnvEnum value { get; set; }
    }
    [Bind(typeof(IEnv))]
    class Env : IEnv
    {
        public EnvEnum value { get; set; }
    }
}
