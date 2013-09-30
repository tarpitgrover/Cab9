using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Cab9.Model;
using Cab9.Hubs.Common;

namespace Cab9.Hubs
{
    public class DriverHub : MasterHub
    {
        public void SendLocation(int driverid, decimal latitude, decimal longitude)
        {
            Driver driver = Driver.SelectByID(driverid);
            driver.LastKnownPosition = new Geography.Point(latitude, longitude);

            if (driver.CurrentShiftID.HasValue)
            {
                DriverShift shift = DriverShift.SelectByID(driver.CurrentShiftID.Value);
                shift.AddNewPoint(new Geography.Point(latitude, longitude));
                shift.Update(false);
                Clients.All.PositionUpdate(new { 
                    DriverID = driver.ID,
                    Point = new Geography.Point(latitude, longitude)
                });
            }

            driver.Update();
        }
    }
}