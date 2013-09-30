using Cab9.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.DataLayer.Common
{
    public class DAL
    {
        internal static string ConnectionString { get { return DALConfig.ConnectionString; } }

        internal static object Safe(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}