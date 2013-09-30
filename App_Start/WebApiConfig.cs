using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Cab9.App_Start
{
    internal static class WebApiConfig
    {
        internal static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "ApiActionRoute",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { action = "DefaultAction" }
            );
        }
    }
}