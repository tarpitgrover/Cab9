using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Security;
using Cab9.Model;
using System.Web.Security;

namespace Cab9.Hubs.Common
{
    public abstract class MasterHub : Hub
    {
        private Identity _user { get; set; }
        public Identity user
        {
            //TODO: May need to get from Cookie
            get
            {
                if (_user == null && Context.RequestCookies.ContainsKey(FormsAuthentication.FormsCookieName))
                    _user = new Identity(User.FromTicket(Context.RequestCookies[FormsAuthentication.FormsCookieName].Value));
                return _user;
            }
        }

        public override Task OnConnected()
        {
            //if (user == null)
            //    throw new SecurityException("User is not logged in");

            Groups.Add(Context.ConnectionId, "1");

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            //if (user == null)
            //    throw new SecurityException("User is not logged in");

            Groups.Remove(Context.ConnectionId, "1");

            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            //if (user == null)
            //    throw new SecurityException("User is not logged in");

            Groups.Add(Context.ConnectionId, "1");

            return base.OnReconnected();
        }
    }
}