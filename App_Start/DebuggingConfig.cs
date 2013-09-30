using e9.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.App_Start
{
    public static class DebuggingConfig
    {
        internal static void Setup()
        {
            Console.SetOut(new DebugTextWriter());
        }
    }
}