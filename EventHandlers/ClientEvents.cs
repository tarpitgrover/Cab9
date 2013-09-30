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
    public static class ClientEvents
    {
        public static IHubContext Hub { get { return GlobalHost.ConnectionManager.GetHubContext<ClientHub>(); } }

        public static void SetupEvents()
        {
            Client.ClientInserted += Client_ClientInserted;
            Client.ClientUpdated += Client_ClientUpdated;
            Client.ClientDeleted += Client_ClientDeleted;
        }

        internal static void Client_ClientDeleted(Client sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).ClientDeleted(sender.ID);
        }

        internal static void Client_ClientUpdated(Client sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).ClientUpdated(sender);
        }

        internal static void Client_ClientInserted(Client sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).ClientInserted(sender);
        }

        public static void TearDownEvents()
        {
            Client.ClientInserted -= Client_ClientInserted;
            Client.ClientUpdated -= Client_ClientUpdated;
            Client.ClientDeleted -= Client_ClientDeleted;
        }
    }
}