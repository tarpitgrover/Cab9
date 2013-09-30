using Cab9.EventHandlers.Common;
using Cab9.Hubs;
using Cab9.Model;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.EventHandlers
{
    public static class DriverShiftEvents
    {
        public static IHubContext Hub { get { return GlobalHost.ConnectionManager.GetHubContext<DriverShiftHub>(); } }

        public static void SetupEvents()
        {
            DriverShift.DriverShiftInserted += DriverShift_DriverShiftInserted;
            DriverShift.DriverShiftUpdated += DriverShift_DriverShiftUpdated;
            DriverShift.DriverShiftDeleted += DriverShift_DriverShiftDeleted;
            DriverShift.DriverShiftRouteUpdated += DriverShift_DriverShiftRouteUpdated;
        }

        static void DriverShift_DriverShiftRouteUpdated(DriverShift sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverShiftRouteUpdated(sender.ID);
        }

        internal static void DriverShift_DriverShiftDeleted(DriverShift sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverShiftDeleted(sender.ID);
        }

        internal static void DriverShift_DriverShiftUpdated(DriverShift sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverShiftUpdated(sender);
        }

        internal static void DriverShift_DriverShiftInserted(DriverShift sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverShiftInserted(sender);
        }

        public static void TearDownEvents()
        {
            DriverShift.DriverShiftInserted -= DriverShift_DriverShiftInserted;
            DriverShift.DriverShiftUpdated -= DriverShift_DriverShiftUpdated;
            DriverShift.DriverShiftDeleted -= DriverShift_DriverShiftDeleted;
        }
    }
}