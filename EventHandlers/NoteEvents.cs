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
    public static class NoteEvents
    {
        public static IHubContext Hub { get { return GlobalHost.ConnectionManager.GetHubContext<NoteHub>(); } }

        public static void SetupEvents()
        {
            Note.NoteInserted += Note_NoteInserted;
            Note.NoteUpdated += Note_NoteUpdated;
            Note.NoteDeleted += Note_NoteDeleted;
        }

        internal static void Note_NoteDeleted(Note sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).NoteDeleted(sender.ID);
        }

        internal static void Note_NoteUpdated(Note sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).NoteUpdated(sender);
        }

        internal static void Note_NoteInserted(Note sender, HubEventArgs e)
        {
            Hub.Clients.Group(e.CompanyID.ToString()).NoteInserted(sender);
        }

        public static void TearDownEvents()
        {
            Note.NoteInserted -= Note_NoteInserted;
            Note.NoteUpdated -= Note_NoteUpdated;
            Note.NoteDeleted -= Note_NoteDeleted;
        }
    }
}