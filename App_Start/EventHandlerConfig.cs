using Cab9.EventHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.App_Start
{
    public class EventHandlerConfig
    {
        public static void Register()
        {
            NoteEvents.SetupEvents();
            DocumentEvents.SetupEvents();
            DriverEvents.SetupEvents();
            ClientEvents.SetupEvents();
            VehicleEvents.SetupEvents();
        }

        public static void Deregister()
        {
            NoteEvents.TearDownEvents();
            DocumentEvents.TearDownEvents();
            DriverEvents.TearDownEvents();
            ClientEvents.TearDownEvents();
            VehicleEvents.TearDownEvents();
        }
    }
}