using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Cab9.Model;
using Cab9.Hubs.Common;
using Cab9.Geography;

namespace Cab9.Hubs
{
    public class TrackingHub : MasterHub
    {
        public void SendUpdate(int shiftid, decimal latitude, decimal longitude, decimal accuracy, decimal speed, decimal heading)
        {
            if (user == null) return;

            var shift = DriverShift.SelectByID(shiftid);

            if (shift != null || shift.CompanyID == user.CompanyID) 
                shift.AddNewPoint(new Point(latitude, longitude));
        }
    }
}