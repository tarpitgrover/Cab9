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
    public static class VehicleEvents
    {
        public static IHubContext Hub { get { return GlobalHost.ConnectionManager.GetHubContext<VehicleHub>(); } }

        public static void SetupEvents()
        {
            Vehicle.VehicleInserted += Vehicle_VehicleInserted;
            Vehicle.VehicleUpdated += Vehicle_VehicleUpdated;
            Vehicle.VehicleDeleted += Vehicle_VehicleDeleted;
            VehicleType.VehicleTypeInserted += VehicleType_VehicleTypeInserted;
            VehicleType.VehicleTypeUpdated += VehicleType_VehicleTypeUpdated;
            VehicleType.VehicleTypeDeleted += VehicleType_VehicleTypeDeleted;
        }

        internal static void VehicleType_VehicleTypeDeleted(VehicleType sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).VehicleTypeDeleted(sender.ID);
        }

        internal static void VehicleType_VehicleTypeUpdated(VehicleType sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).VehicleTypeUpdated(sender.ID);
        }

        internal static void VehicleType_VehicleTypeInserted(VehicleType sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).VehicleTypeInserted(sender.ID);
        }

        internal static void Vehicle_VehicleDeleted(Vehicle sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).VehicleDeleted(sender.ID);
        }

        internal static void Vehicle_VehicleUpdated(Vehicle sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).VehicleUpdated(sender);
        }

        internal static void Vehicle_VehicleInserted(Vehicle sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).VehicleInserted(sender);
        }

        public static void TearDownEvents()
        {
            Vehicle.VehicleInserted -= Vehicle_VehicleInserted;
            Vehicle.VehicleUpdated -= Vehicle_VehicleUpdated;
            Vehicle.VehicleDeleted -= Vehicle_VehicleDeleted;
            VehicleType.VehicleTypeInserted -= VehicleType_VehicleTypeInserted;
            VehicleType.VehicleTypeUpdated -= VehicleType_VehicleTypeUpdated;
            VehicleType.VehicleTypeDeleted -= VehicleType_VehicleTypeDeleted;

        }
    }
}