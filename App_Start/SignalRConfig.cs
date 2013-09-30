using Microsoft.Owin;
using Owin;
using System.Web.Routing;

namespace Cab9.App_Start
{
    public class SignalRConfig
    {
        public static void ManualRegister(RouteCollection routes)
        {
            routes.MapHubs();
        }

        public void Configuration(IAppBuilder builder)
        {
            builder.MapSignalR();
        }
    }
}