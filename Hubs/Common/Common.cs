using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Security;
using Cab9.Model;
using System.Web.Security;
using System.Collections.Concurrent;

namespace Cab9.Hubs.Common
{
        [Flags]
        public enum Subscriptions
        {
            Basic,
            DriverInsert,
            DriverUpdate,
            DriverDelete,
            DriverTracking
        }

        public class SignalrUser
        {
            public string Username { get; set; }
            public int CompanyId { get; set; }
            public HashSet<string> ConnectionIds { get; set; }
            public Subscriptions Subscriptions { get; set; }

            public SignalrUser(string name, int companyid)
            {
                Subscriptions = Subscriptions.Basic;
                Username = name;
                CompanyId = companyid;
                ConnectionIds = new HashSet<string>();
            }
        }
}