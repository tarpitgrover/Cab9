using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Cab9.Model;
using Cab9.Hubs.Common;
using Cab9.Geography;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Web.Security;

namespace Cab9.Hubs
{
    public class DriverHub : Hub
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

        private static ConcurrentDictionary<string, SignalrUser> SignalrUsers = new ConcurrentDictionary<string, SignalrUser>();

        public override Task OnConnected()
        {
            //if (user == null)
            //    throw new SecurityException("User is not logged in");

            //string connection = Context.ConnectionId;

            //var sUser = SignalrUsers.GetOrAdd(user.Name, new SignalrUser(user.Name, user.CompanyID));

            //lock (sUser.ConnectionIds)
            //{
            //    sUser.ConnectionIds.Add(connection);
            //}

            Groups.Add(Context.ConnectionId, "1");

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            //if (user == null)
            //    throw new SecurityException("User is not logged in");

            Groups.Remove(Context.ConnectionId, "1");

            //SignalrUser sUser;
            //SignalrUsers.TryGetValue(user.Name, out sUser);

            //if (sUser != null)
            //{

            //    lock (sUser.ConnectionIds)
            //    {

            //        sUser.ConnectionIds.RemoveWhere(cid => cid.Equals(Context.ConnectionId));

            //        if (!sUser.ConnectionIds.Any())
            //        {
            //            SignalrUser removedUser;
            //            SignalrUsers.TryRemove(user.Name, out removedUser);
            //        }
            //    }
            //}

            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void SendUpdate(int shiftid, decimal? latitude, decimal? longitude, decimal? accuracy, decimal? speed, decimal? heading)
        {
            if (user == null) return;

            var shift = DriverShift.SelectByID(shiftid);

            if (shift != null && shift.CompanyID == user.CompanyID && latitude.HasValue && longitude.HasValue) 
                shift.AddNewPoint(new Point(latitude.Value, longitude.Value));
        }

        public void AcceptBookingOffer(long offerid)
        {

        }

        public void RejectBookingOffer(long offerid, string reason)
        {
        }

    }
}