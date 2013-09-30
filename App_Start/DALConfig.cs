using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.App_Start
{
    public class DALConfig
    {
        
#if DEBUG
        public const string ConnectionString = @"Server=e9server.com; Database=Cab9DbV1; User Id=sa; Password=E9inetarp1t;  Max Pool Size=5024;";
        public const string WebRoot = @"http://localhost:50808/";
#elif LIGHTDEBUG
        public const string ConnectionString = @"Server=E9INEPC2\MSSQLSERVER2008; Database=Cab9Db; User Id=sa; Password=E9inetarp1t!";
#else
        public const string ConnectionString = @"Server=e9server.com; Database=Cab9Db; User Id=sa; Password=E9inetarp1t;  Max Pool Size=5024;";
#endif

        public static void Config()
        {
        }
    }
}