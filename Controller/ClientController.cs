using Cab9.Controller.Common;
using Cab9.Model;
using Cab9.Common;
using Cab9.Model.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using Cab9.Stats;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;

namespace Cab9.Controller
{
    public class ClientController : Cab9ApiController
    {
        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(int? clientid = null, bool? active = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            var result = Client.Select(clientid, CompanyID.Value, active);
            
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [ActionName("GetByID")]
        public HttpResponseMessage GetByID(int clientid)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (clientid < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Client id");

            var result = Client.SelectByID(clientid);
            if (result == null || result.CompanyID != CompanyID.Value)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Client could not be found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            
        }

        [HttpPost]
        [ActionName("Post")]
        public HttpResponseMessage Post([FromBody]Client value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Client object was not supplied.");

            if (value.ID > 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "Client object already has ID, check Client is not already saved.");

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
        [ActionName("Put")]
        public HttpResponseMessage Put([FromBody]Client value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Client object was not supplied.");
            
            var result = Client.SelectByID(value.ID);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Client could not be found.");

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
        public HttpResponseMessage Delete(int id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Client id");

            var result = Client.SelectByID(id);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Client could not be found.");

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
        public async Task<HttpResponseMessage> PostWithImage()
        {
            /////////   Checks
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            /////////   Read to Temp
            var root = HttpContext.Current.Server.MapPath("~/App_Data/Temp/FileUploads");
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            ////////    Save Model
            if (result.FormData["model"] == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var model = result.FormData["model"];
            Client client = JsonConvert.DeserializeObject<Client>(model);
            client.CompanyID = CompanyID.Value;
            var response = Post(client);
            if (response.StatusCode != HttpStatusCode.Created) return response;


            ////////    Save Uploads (if present)
            if (result.FormData["newfile"] == "true")
            {
                foreach (var file in result.FileData)
                {
                    string url;
                    if (TryMoveUpload(file, "/Uploads/Client/Logos", true, out url))
                    {
                        client.LogoURL = url;
                        client.Update();
                    }
                }
            }
            return Request.CreateResponse<Client>(HttpStatusCode.Created, client);
        }

        [HttpPut]
        [ActionName("DefaultAction")]
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
            Client client = JsonConvert.DeserializeObject<Client>(model);
            client.CompanyID = CompanyID.Value;
            var response = Put(client);
            if (response.StatusCode != HttpStatusCode.OK) return response;

            if (result.FormData["newfile"] == "true")
            {
                //get the files
                foreach (var file in result.FileData)
                {
                    string url;
                    if (TryMoveUpload(file, "/Uploads/Client/Logos", true, out url))
                    {
                        DeleteOldDownload(client.LogoURL);
                        client.LogoURL = url;
                        client.Update();
                    }
                }
            }
            return Request.CreateResponse<Client>(HttpStatusCode.OK, client);
        }

        [HttpGet]
        [ActionName("Stats")]
        public HttpResponseMessage Stats(int clientId = 0, DateTime? from = null, DateTime? to = null, string grouping = "Smart", string timeformat = "utc")
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");
            if (clientId < -2) return Request.CreateResponse(HttpStatusCode.BadRequest, "ClientID value not valid, requires valid client id, -1 for averages or -2 for ideals.");
            var client = Client.SelectByID(clientId);
            if (clientId > 0 && (client == null || client.CompanyID != CompanyID.Value)) return Request.CreateResponse(HttpStatusCode.BadRequest, "ClientID value not valid, requires valid client id, -1 for averages or -2 for ideals.");
            var i = ClientStatsFactory.Generate(clientId, CompanyID.Value, from, to, grouping, timeformat);
            return Request.CreateResponse(HttpStatusCode.OK, i);
        }


    }
}