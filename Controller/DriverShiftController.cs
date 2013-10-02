using Cab9.Controller.Common;
using Cab9.Model;
using Cab9.Model.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using Cab9.Geography;

namespace Cab9.Controller
{
    public class DriverShiftController : Cab9ApiController
    {
        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(long? id = null, int? driverId = null, int? vehicleId = null, bool? active = null, DateTime? from = null, DateTime? to = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            var result = DriverShift.Select(id, CompanyID.Value, driverId, vehicleId, active, from, to);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [ActionName("GetByID")]
        public HttpResponseMessage GetByID(long id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid DriverShift id");

            var result = DriverShift.SelectByID(id);
            if (result == null || result.CompanyID != CompanyID.Value)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "DriverShift could not be found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            
        }


        [HttpPost]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Post([FromBody]DriverShift value)
        {
            return StartShift(value);
        }

        [HttpPost]
        public HttpResponseMessage StartShift([FromBody]DriverShift value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "DriverShift object was not supplied.");

            if (value.ID > 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "DriverShift object already has ID, check DriverShift is not already saved.");

            Driver driver = null;
            if (value.DriverID != null)
            {
                driver = Driver.SelectByID(value.DriverID);
                if (driver == null || driver.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.BadRequest, "Driver ID incorrect.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Driver ID Missing.");
            }

            value.CompanyID = CompanyID.Value;
            value.ShiftStart = DateTime.Now;
            value.EncodedRoute = "";

            var success = value.Insert();
            if (success)
            {
                driver.Status = DriverStatus.Available;
                driver.CurrentShiftID = value.ID;
                driver.Update();
                return Request.CreateResponse(HttpStatusCode.Created, value);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occured, please check your input and try again.");
            }
        }

        [HttpPut]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Put([FromBody]DriverShift value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "DriverShift object was not supplied.");
            
            var result = DriverShift.SelectByID(value.ID);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "DriverShift could not be found.");

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

        [HttpPut]
        public HttpResponseMessage EndShift([FromBody]DriverShift value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "DriverShift object was not supplied.");

            Driver driver = null;
            if (value.DriverID != null)
            {
                driver = Driver.SelectByID(value.DriverID);
                if (driver == null || driver.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.BadRequest, "Driver ID incorrect.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Driver ID Missing.");
            }

            value.CompanyID = CompanyID.Value;
            value.ShiftEnd = DateTime.Now;

            var success = value.Update();
            if (success)
            {
                driver.CurrentShiftID = null;
                driver.Status = DriverStatus.OffDuty;
                driver.Update();

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

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid DriverShift id");

            var result = DriverShift.SelectByID(id);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "DriverShift could not be found.");

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

        [HttpPost]
        [ActionName("DefaultAction")]
        public HttpResponseMessage PostPoint(int shiftId, [FromBody]Point value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Point object was not supplied.");

            var shift = DriverShift.SelectByID(shiftId);

            if (shift == null || shift.CompanyID != CompanyID) return Request.CreateResponse(HttpStatusCode.BadRequest, "DriverShift not found.");

            var success = shift.AddNewPoint(value);
            if (success)
            {
                return Request.CreateResponse(HttpStatusCode.Created, value);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occured, please check your input and try again.");
            }
        }

        [HttpPost]
        [ActionName("DefaultAction")]
        public HttpResponseMessage PostPoint(int shiftId, decimal latitude, decimal longitude)
        {
            return PostPoint(shiftId, new Point(latitude, longitude));
        }
    }
}