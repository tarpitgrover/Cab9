using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cab9.EventHandlers.Common
{
    public class HubEventArgs : EventArgs
    {
        public int CompanyID { get; set; }
        public int UserId { get; set; }

        public HubEventArgs(int companyID, int userID)
        {
            CompanyID = companyID;
            UserId = userID;
        }
    }
}