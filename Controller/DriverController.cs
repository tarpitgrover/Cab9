using Cab9.Controller.Common;
using Cab9.Model;
using Cab9.Model.Common;
using Cab9.Stats;
using Cab9.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Cab9.App_Start;

namespace Cab9.Controller
{
    public class DriverController : Cab9ApiController
    {
        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(int? driverid = null, string callSign = null, string mobile = null, string telephone = null, string email = null, bool? active = null, DateTime? dateOfBirth = null, int? DriverType = null, int? status = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            var result = Driver.Select(driverid, CompanyID.Value, callSign, mobile, telephone, email, active, dateOfBirth, DriverType, status);

            
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [ActionName("GetByID")]
        public HttpResponseMessage GetByID(int driverid)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (driverid < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Driver id");

            var result = Driver.SelectByID(driverid);
            if (result == null || result.CompanyID != CompanyID.Value)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Driver could not be found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            
        }

        [HttpPost]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Post([FromBody]Driver value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Driver object was not supplied.");

            if (value.ID > 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "Driver object already has ID, check Driver is not already saved.");

            value.CompanyID = CompanyID.Value;
            value.Active = true;

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
        public HttpResponseMessage Put([FromBody]Driver value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Driver object was not supplied.");
            
            var result = Driver.SelectByID(value.ID);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Driver could not be found.");

            value.CompanyID = CompanyID.Value;
            var success = value.Update();
            if (success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, value);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occured, please check your input and try again.");
            }
        }

        [HttpDelete]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Delete(int driverid)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (driverid < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Driver id");

            var result = Driver.SelectByID(driverid);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Driver could not be found.");

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
        [ActionName("PostWithImage")]
        public async Task<HttpResponseMessage> PostWithImage()
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var root = HttpContext.Current.Server.MapPath("~/App_Data/Temp/FileUploads");
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            if (result.FormData["model"] == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var model = result.FormData["model"];
            Driver driver = JsonConvert.DeserializeObject<Driver>(model);
            driver.CompanyID = CompanyID.Value;
            var response = Post(driver);
            if (response.StatusCode != HttpStatusCode.Created) return response;

            //get the files
            if (result.FormData["newpicture"] == "true")
            {
                //get the files
                foreach (var file in result.FileData)
                {
                    string url;
                    if (TryMoveUpload(file, "/Uploads/Driver/Images", true, out url))
                    {
                        DeleteOldDownload(driver.ImageUrl);
                        driver.ImageUrl = url;
                        driver.Update();
                    }
                }
            }
            return Request.CreateResponse<Driver>(HttpStatusCode.OK, driver);
        }

        [HttpPut]
        [ActionName("PutWithImage")]
        public async Task<HttpResponseMessage> PutWithImage()
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var root = HttpContext.Current.Server.MapPath("~/App_Data/Temp/FileUploads");
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            if (result.FormData["model"] == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var model = result.FormData["model"];
            Driver driver = JsonConvert.DeserializeObject<Driver>(model);
            var response = Put(driver);
            if (response.StatusCode != HttpStatusCode.OK) return response;

            if (result.FormData["newpicture"] == "true")
            {
                //get the files
                foreach (var file in result.FileData)
                {
                    string url;
                    if (TryMoveUpload(file, "/Uploads/Driver/Images", true, out url))
                    {
                        DeleteOldDownload(driver.ImageUrl);
                        driver.ImageUrl = url;
                        driver.Update();
                    }
                }
            }
            return Request.CreateResponse<Driver>(HttpStatusCode.OK, driver);
        }

        [HttpGet]
        [ActionName("Stats")]
        public HttpResponseMessage Stats(int driverid = 0, DateTime? from = null, DateTime? to = null, string grouping = "Smart", string timeformat = "utc")
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");
            if (driverid < -2) return Request.CreateResponse(HttpStatusCode.BadRequest, "DriverID value not valid, requires valid driver id, -1 for averages or -2 for ideals.");
            var driver = Driver.SelectByID(driverid);
            if (driverid > 0 && (driver == null || driver.CompanyID != CompanyID.Value)) return Request.CreateResponse(HttpStatusCode.BadRequest, "DriverID value not valid, requires valid driver id, -1 for averages or -2 for ideals.");
            var i = DriverStatsFactory.Generate(driverid, CompanyID.Value, from, to, grouping, timeformat);
            return Request.CreateResponse(HttpStatusCode.OK, i);
        }
    }
}