using Cab9.Controller.Common;
using Cab9.Model;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace Cab9.Controller
{
    public class PricingZoneController : Cab9ApiController
    {
        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(int? id = null, int? pricingmodelid = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            List<PricingZone> result = PricingZone.Select(id, CompanyID, pricingmodelid);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [ActionName("GetByID")]
        public HttpResponseMessage GetByID(int id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid PricingZone id");

            var result = PricingZone.SelectByID(id);
            if (result == null || result.CompanyID != CompanyID.Value)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "PricingZone could not be found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            
        }

        [HttpPost]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Post([FromBody]PricingZone value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "PricingZone object was not supplied.");

            //if (value.ID > 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "PricingZone object already has ID, check PricingZone is not already saved.");

            value.CompanyID = CompanyID.Value;

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
        public HttpResponseMessage Put([FromBody]PricingZone value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "PricingZone object was not supplied.");
            
            var result = PricingZone.SelectByID(value.ID);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "PricingZone could not be found.");

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
        public HttpResponseMessage Delete(int id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid PricingZone id");

            var result = PricingZone.SelectByID(id);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "PricingZone could not be found.");
 
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
    }
}