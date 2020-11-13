using NatCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NatCenter
{
    [Bind(typeof(ITestLog))]
    class ConsoleLog : ITestLog
    {
        public void Log(object msg)
        {
            Console.WriteLine($"{DateTime.Now}  Log::{msg}");
        }
    }
}
