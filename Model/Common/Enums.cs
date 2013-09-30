using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.Model.Common
{
    public enum DriverStatus
    {
        Undefined = -1,
        OffDuty = 0,
        Unavailable = 1,
        PickingUp = 2,
        OnJob = 3,
        Clearing = 4,
        OnBreak = 5,
        Available = 6
    }

    public enum InvoicePeriod
    {
        Unspecified = -1,
        Manually = 0,
        JobByJob = 1,
        Daily = 2,
        Weekly = 3,
        BiWeekly = 4,
        Monthly = 5
    }

    public enum InvoiceMethod
    {
        Unspecified = -1,
        Other = 0,
        EmailOnly = 1,
        PostOnly = 2,
        Both = 3,
    }

    public enum ZoneMode
    {
        NoZones = 0,
        HighestEntered = 1,
        LastEntered = 2,
        AverageEntered = 3
    }

    public enum BookingStatus
    {
        NoShow = -3,
        Cancelled = -2,
        Unspecified = -1,
        Unconfirmed = 0,
        Confirmed = 1,
        Assigned = 2,
        Dispatched = 3,
        Waiting = 4,
        OnBoard = 5,
        Clearing = 6,
        Completed = 7
    }

    public enum BookingPriority
    {
        Unspecified = -1,
        Low = 0,
        LowMedium = 1,
        Medium = 2,
        MediumHigh = 3,
        High = 4,
        VIP = 5
    }
}