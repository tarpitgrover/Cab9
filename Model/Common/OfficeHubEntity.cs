using Cab9.Hubs;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Model.Common
{
    public class OfficeHubEntity
    {
        public static IHubContext OfficeHub { get { return GlobalHost.ConnectionManager.GetHubContext<OfficeHub>(); } }
    }
}