using Cab9.Api.Compression;
using Cab9.App_Start;
using Cab9.Common;
using Cab9.DataLayer;
using Cab9.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Cab9
{
    public class Global : HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            DebuggingConfig.Setup();

            //SignalRConfig.Register(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configuration.MessageHandlers.Add(new OptionsRequestHandler());
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new CompressionHandler());

            SecurityConfig.Register(GlobalConfiguration.Configuration);
            MediaFormattersConfig.Register(GlobalConfiguration.Configuration.Formatters);

            DALConfig.Config();
            EventHandlerConfig.Register();

            TimeSpan Start = new TimeSpan(0, 0, 4), Period = new TimeSpan(0, 0, 4);
            LiveDemo live = new LiveDemo(Start, Period);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "GET" || Request.HttpMethod == "POST" || Request.HttpMethod == "PUT" || Request.HttpMethod == "DELETE")
            {
                if (Request.Headers.AllKeys.Contains("Origin"))
                    Response.Headers.Add("Access-Control-Allow-Origin", Request.Headers.GetValues("Origin").First());
                else
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE");
                Response.Headers.Add("Access-Control-Max-Age", "10000");
                Response.Headers.Add("Access-Control-Allow-Headers", "X-Requested-With, Content-Type, ApiUserKey, Authorization");
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}