using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Cab9.Model;
using Cab9.Hubs.Common;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using Cab9.Model.Common;

namespace Cab9.Hubs
{
    public class DispatchHub : MasterHub
    {
        public void AcceptBooking(long offerId)
        {

        }

        public void RejectBooking(long offerId, int reason)
        {

        }
    }
}