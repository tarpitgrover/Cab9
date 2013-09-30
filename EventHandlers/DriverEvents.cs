using Cab9.EventHandlers.Common;
using Cab9.Hubs;
using Cab9.Model;
using Microsoft.AspNet.SignalR;

namespace Cab9.EventHandlers
{
    public static class DriverEvents
    {
        public static IHubContext Hub { get { return GlobalHost.ConnectionManager.GetHubContext<DriverHub>(); } }

        public static void SetupEvents()
        {
            Driver.DriverInserted += Driver_DriverInserted;
            Driver.DriverUpdated += Driver_DriverUpdated;
            Driver.DriverDeleted += Driver_DriverDeleted;
            DriverType.DriverTypeInserted += DriverType_DriverTypeInserted;
            DriverType.DriverTypeUpdated += DriverType_DriverTypeUpdated;
            DriverType.DriverTypeDeleted += DriverType_DriverTypeDeleted;
        }

        internal static void DriverType_DriverTypeDeleted(DriverType sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverTypeDeleted(sender);
        }

        internal static void DriverType_DriverTypeUpdated(DriverType sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverTypeUpdated(sender);
        }

        internal static void DriverType_DriverTypeInserted(DriverType sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverTypeInserted(sender);
        }

        internal static void Driver_DriverDeleted(Driver sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverDeleted(sender.ID);
        }

        internal static void Driver_DriverUpdated(Driver sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverUpdated(sender);
        }

        internal static void Driver_DriverInserted(Driver sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DriverInserted(sender);
        }

        public static void TearDownEvents()
        {
            Driver.DriverInserted -= Driver_DriverInserted;
            Driver.DriverUpdated -= Driver_DriverUpdated;
            Driver.DriverDeleted -= Driver_DriverDeleted;
        }
    }
}