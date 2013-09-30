using Cab9;
using Cab9.Handlers;
using System.Web.Http;
using System.Web.Security;

namespace Cab9.App_Start
{
    public class SecurityConfig
    {
#if DEBUG
        public const string SecurityConnectionString = @"Server=e9server.com; Database=Cab9DbV1; User Id=sa; Password=E9inetarp1t;  Max Pool Size=5024;";
#elif LIGHTDEBUG
        public const string SecurityConnectionString = @"Server=E9INEPC2\MSSQLSERVER2008; Database=Cab9Db; User Id=sa; Password=E9inetarp1t!";
#else
        public const string SecurityConnectionString = @"Server=e9server.com; Database=Cab9Db; User Id=sa; Password=E9inetarp1t;  Max Pool Size=5024;";
#endif

        internal static void Register(HttpConfiguration config)
        {
            FormsAuthentication.Initialize();
            Cab9Config.Initialise(SecurityConnectionString, "Cab9", 3);
            config.MessageHandlers.Add(new AuthenticationHandler());
        }
    }
}