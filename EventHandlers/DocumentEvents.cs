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
    public static class DocumentEvents
    {
        public static IHubContext Hub { get { return GlobalHost.ConnectionManager.GetHubContext<DocumentHub>(); } }

        public static void SetupEvents()
        {
            Document.DocumentInserted += Document_DocumentInserted;
            Document.DocumentUpdated += Document_DocumentUpdated;
            Document.DocumentDeleted += Document_DocumentDeleted;
        }

        internal static void Document_DocumentDeleted(Document sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DocumentDeleted(sender.ID);
        }

        internal static void Document_DocumentUpdated(Document sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DocumentUpdated(sender);
        }

        internal static void Document_DocumentInserted(Document sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).DocumentInserted(sender);
        }

        public static void TearDownEvents()
        {
            Document.DocumentInserted -= Document_DocumentInserted;
            Document.DocumentUpdated -= Document_DocumentUpdated;
            Document.DocumentDeleted -= Document_DocumentDeleted;
        }
    }
}