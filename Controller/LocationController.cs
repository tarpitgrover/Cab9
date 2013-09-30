using Cab9.Controller.Common;
using Cab9.Model;
using Cab9.Model.Common;
using Cab9.Common;
using GoogleMapsApi.Entities.Places.Request;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using GoogleMapsApi.Engine;

namespace Cab9.Controller
{
    public class LocationController : Cab9ApiController
    {
        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(int? id = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            var result = Location.Select(id, CompanyID.Value);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }        
        
        //[HttpGet]
        //[ActionName("Search")]
        //public HttpResponseMessage Search(string q)
        //{
        //    if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

        //    q = q.Replace('+', ' ');

        //    if (q.IsShortPostcode()) return Request.CreateResponse(HttpStatusCode.OK, new string[]{});


        //    var queryWords = q.Split(' ');

        //    var airportModifier = (queryWords.Any(x => x.MatchPercentage("Airport") > 0.8M)) ? 2 : 1;
        //    var trainModifier = (queryWords.Any(x => x.MatchPercentage("Train") > 0.8M)) ? 2 : 1;
        //    var stationModifier = (queryWords.Any(x => x.MatchPercentage("Station") > 0.8M)) ? 2 : 1;
        //    var undergroundModifier = (queryWords.Any(x => x.MatchPercentage("Underground") > 0.8M)) ? 2 : 1;

        //    List<Location> allResults = Location.Select(companyId: CompanyID);

        //    var preped = allResults.Select(x =>
        //    {
        //        var match = x.Searchable.MatchPercentage(q);
        //        if (x.Type == "Airport") match *= airportModifier;
        //        if (x.Type == "Train Station") match *= trainModifier * stationModifier;
        //        if (x.Type == "Underground Station") match *= undergroundModifier * stationModifier;
        //        if (x.Name.StartsWith(queryWords[0], StringComparison.InvariantCultureIgnoreCase) || x.Searchable.StartsWith(queryWords[0], StringComparison.InvariantCultureIgnoreCase)) { match *= 2; }
        //        if (queryWords.Length > 1 && (x.Name.StartsWith(queryWords[0] + ' ' + queryWords[1], StringComparison.InvariantCultureIgnoreCase) || x.Searchable.StartsWith(queryWords[0] + ' ' + queryWords[1], StringComparison.InvariantCultureIgnoreCase))) { match *= 2; }

        //        return new { x.Name, x.Type, x.Latitude, x.Longitude, Match = match };
        //    }).OrderByDescending(x => x.Match).Take(20).ToList();

        //    var filtered = preped.GroupBy(x => x.Type).Select(x => x.Take(5)).SelectMany(x => x.ToList()).OrderByDescending(x => x.Match);

        //    if (filtered.Count() > 0 && filtered.First().Match >= 1.5M) return Request.CreateResponse(HttpStatusCode.OK, filtered.Where(x => x.Match >= 0.75M));

        //    return Request.CreateResponse(HttpStatusCode.OK, filtered);
        //}

        [HttpGet]
        [ActionName("Search")]
        public HttpResponseMessage Search(string q)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            q = q.Replace('+', ' ');

            if (q.IsShortPostcode()) return Request.CreateResponse(HttpStatusCode.OK, new string[] { });


            var queryWords = q.Split(' ');

            var airportModifier = (queryWords.Any(x => x.MatchPercentage("Airport") > 0.8M)) ? 2 : 1;
            var trainModifier = (queryWords.Any(x => x.MatchPercentage("Train") > 0.8M)) ? 2 : 1;
            var stationModifier = (queryWords.Any(x => x.MatchPercentage("Station") > 0.8M)) ? 2 : 1;
            var undergroundModifier = (queryWords.Any(x => x.MatchPercentage("Underground") > 0.8M)) ? 2 : 1;

            List<Location> allResults = Location.Select(companyId: CompanyID, search: queryWords[0]);

            var preped = allResults.Select(x =>
            {
                var match = x.Searchable.MatchPercentage(q);
                if (x.Type == "Airport") match *= airportModifier;
                if (x.Type == "Train Station") match *= trainModifier * stationModifier;
                if (x.Type == "Underground Station") match *= undergroundModifier * stationModifier;
                if (x.Name.StartsWith(queryWords[0], StringComparison.InvariantCultureIgnoreCase) || x.Searchable.StartsWith(queryWords[0], StringComparison.InvariantCultureIgnoreCase)) { match *= 2; }
                if (queryWords.Length > 1 && (x.Name.StartsWith(queryWords[0] + ' ' + queryWords[1], StringComparison.InvariantCultureIgnoreCase) || x.Searchable.StartsWith(queryWords[0] + ' ' + queryWords[1], StringComparison.InvariantCultureIgnoreCase))) { match *= 2; }

                return new { x.Name, x.Type, x.Latitude, x.Longitude, x.Searchable, x.Note, Match = match };
            }).OrderByDescending(x => x.Match).Take(20).ToList();

            var filtered = preped.GroupBy(x => x.Type).Select(x => x.Take(5)).SelectMany(x => x.ToList()).OrderByDescending(x => x.Match);

            if (queryWords.Length > 1 && filtered.Any(x => x.Name.StartsWith(queryWords[0] + " " + queryWords[1], StringComparison.InvariantCultureIgnoreCase))) return Request.CreateResponse(HttpStatusCode.OK, filtered.Where(x => x.Name.StartsWith(queryWords[0] + " " + queryWords[1], StringComparison.InvariantCultureIgnoreCase)));
            if (queryWords.Length > 1 && filtered.Any(x => x.Searchable.StartsWith(queryWords[0] + " " + queryWords[1], StringComparison.InvariantCultureIgnoreCase))) return Request.CreateResponse(HttpStatusCode.OK, filtered.Where(x => x.Searchable.StartsWith(queryWords[0] + " " + queryWords[1], StringComparison.InvariantCultureIgnoreCase)));

            return Request.CreateResponse(HttpStatusCode.OK, filtered);
        }

        [HttpGet]
        [ActionName("GetByID")]
        public HttpResponseMessage GetByID(int id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Location id");

            var result = Location.SelectByID(id);
            if (result == null || result.CompanyID != CompanyID.Value)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Location could not be found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            
        }

        [HttpPost]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Post([FromBody]Location value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Location object was not supplied.");

            if (value.ID > 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "Location object already has ID, check Location is not already saved.");

            if (value.Insert())
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
        public HttpResponseMessage Put([FromBody]Location value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Location object was not supplied.");
            
            var result = Location.SelectByID(value.ID);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Location could not be found.");

            if (value.Update())
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

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Location id");

            var result = Location.SelectByID(id);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Location could not be found.");

            if (result.Delete())
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