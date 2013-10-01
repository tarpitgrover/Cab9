using Cab9.Controller.Common;
using Cab9.Model;
using Cab9.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using Cab9.Stats;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Directions.Response;
using GoogleMapsApi.Entities.Directions.Request;
using Cab9.Geography;

namespace Cab9.Controller
{
    public class BookingController : Cab9ApiController
    {
        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(long? id = null, int? priority = null, int? status = null, int? invoiceID = null, long? shiftID = null, DateTime? bookedFrom = null, DateTime? bookedTo = null, int? clientID = null, int? carType = null, int? driverID = null, int? vehicleID = null, int? paymentMethod = null, int? bookedBy = null, int? editedBy = null, DateTime? leadFrom = null, DateTime? leadTo = null, bool? autoDispatch = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");
            
            var result = Booking.Select(id, CompanyID.Value, priority, status, invoiceID, shiftID, bookedFrom, bookedTo, clientID, carType, driverID, vehicleID, paymentMethod, bookedBy, editedBy, leadFrom, leadTo, autoDispatch);
            
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        [HttpGet]
        [ActionName("GetByID")]
        public HttpResponseMessage GetByID(long id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Booking id");

            var result = Booking.SelectByID(id);
            if (result == null || result.CompanyID != CompanyID.Value)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Booking could not be found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            
        }

        [HttpPost]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Post([FromBody]Booking value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Booking object was not supplied.");

            if (value.ID > 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "Booking object already has ID, check Booking is not already saved.");

            value.CompanyID = CompanyID.Value;
            value.Timestamp = DateTime.Now;
            value.LeadTime = DateTime.Now;

            var success = value.Insert();
            if (success)
            {
                return Request.CreateResponse(HttpStatusCode.Created, value);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occured, please check your input and try again.");
            }
        }

        [HttpPut]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Put([FromBody]Booking value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Booking object was not supplied.");
            
            var result = Booking.SelectByID(value.ID);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Booking could not be found.");

            value.CompanyID = CompanyID.Value;
            var success = value.Update();
            if (success)
            {
                return Request.CreateResponse(HttpStatusCode.Created, value);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occured, please check your input and try again.");
            }
        }

        [HttpDelete]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Delete(long id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Booking id");

            var result = Booking.SelectByID(id);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Booking could not be found.");

            var success = result.Delete();
            if (success)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occured, please check your input and try again.");
            }
        }

        [HttpGet]
        public HttpResponseMessage DriverOrderForQuote(decimal latitude, decimal longitude, int? pax = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            var company = Company.SelectByID(CompanyID.Value);
            var drivers = Driver.Select(companyId: CompanyID.Value, active:true);
            drivers = drivers.Where(d => (d.Status == DriverStatus.Available || d.Status == DriverStatus.Clearing) && (d.CurrentShiftID.HasValue)).ToList();

            if (drivers.Count == 0) return Request.CreateResponse(HttpStatusCode.OK, new object[1]);

            var gmapRequest = new DirectionsRequest();

            gmapRequest.Destination = new Point(latitude, longitude).ToString();

            List<dynamic> result = new List<dynamic>();

            foreach (var d in drivers)
            {
                int
                    PointsDistance = 1000,
                    PointsClearing = (d.Status == DriverStatus.Clearing) ? company.ClearingModifier : 0,
                    PointsShift = 0;
                decimal distance = 0;

                if (d.LastKnownPosition != null)
                {
                    if (d.Status == DriverStatus.Clearing && d.CurrentBookingID.HasValue)
                    {
                        Booking currentBooking = Booking.SelectByID(d.CurrentBookingID.Value);
                        gmapRequest.Origin = d.LastKnownPosition.ToString();
                        gmapRequest.Waypoints = new string[] { currentBooking.To };
                        var response = GoogleMaps.Directions.Query(gmapRequest);
                        if (response.Status == DirectionsStatusCodes.OK)
                        {
                            distance = response.Routes.First().Legs.Sum(l => l.Distance.Value * 0.00062137M);
                            PointsDistance = Convert.ToInt32(company.DistanceModifier * distance);
                        }
                    }
                    else
                    {
                        gmapRequest.Origin = d.LastKnownPosition.ToString();
                        var response = GoogleMaps.Directions.Query(gmapRequest);
                        if (response.Status == DirectionsStatusCodes.OK)
                        {
                            distance = response.Routes.First().Legs.First().Distance.Value * 0.00062137M;
                            PointsDistance = Convert.ToInt32(company.DistanceModifier * distance);
                        }
                    }
                }

                //DriverShift shift = DriverShift.SelectByID(d.CurrentShiftID ?? 0);
                //if (shift != null)
                //{
                //    var count = shift.GetBookings().Count();
                //    PointsShift = count * company.ShiftBookingsModifier;
                //}

                result.Add(new
                {
                    driver = d,
                    //vehicle = (shift != null) ? ((shift.Vehicle != null) ? shift.Vehicle : null) : null,
                    distance = distance,
                    total = PointsClearing + PointsDistance + PointsShift
                });
            }

            if (result.Count == 0) return Request.CreateResponse(HttpStatusCode.OK, new object[1]);

            //result = result.OrderBy(x => x.total).Where(x => (x.vehicle != null) ? (x.vehicle.PAX >= pax) : false).ToList();
            result = result.OrderBy(x => x.total).ToList();

            if (result.Count == 0) return Request.CreateResponse(HttpStatusCode.OK, new object[1]);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage Quote(string encodedRoute, string waitingTimes = null, int? carType = null, int? clientId = null, int? pricingModel = null, DateTime? bookingtime = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            Quote quote = new Quote();
            quote.PopulateQuote(CompanyID.Value, encodedRoute, waitingTimes, carType, clientId, pricingModel, bookingtime);
            return Request.CreateResponse(HttpStatusCode.OK, quote);
        }

        [HttpGet]
        [ActionName("SearchPrevious")]
        public HttpResponseMessage SearchPrevious(string name = null, string number = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            var result = Booking.Search(CompanyID.Value, name, number);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}