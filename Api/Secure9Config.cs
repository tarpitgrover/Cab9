using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cab9
{
    public static class Cab9Config
    {
        internal static string ConnectionString { get; set; }
        internal static string ApplicationName { get; set; }
        internal static int MaxRetries { get; set; }

        public static void Initialise(string connectionString, string applicationName, int maxAttempts)
        {
            ConnectionString = connectionString;
            ApplicationName = applicationName;
            MaxRetries = maxAttempts;
        }
    }
}
