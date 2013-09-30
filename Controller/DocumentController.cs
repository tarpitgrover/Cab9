using Cab9.Controller.Common;
using Cab9.Model;
using Cab9.Model.Common;
using e9.Debugging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Cab9.Controller
{
    public class DocumentController : Cab9ApiController
    {
        [HttpGet]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Get(int? id = null, string documentType = null, string ownerType = null, int? ownerId = null, DateTime? expiryFrom = null, DateTime? expiryTo = null, string identificationNo = null)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            var result = Document.Select(id, CompanyID.Value, documentType, ownerType, ownerId, expiryFrom, expiryTo, identificationNo);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [ActionName("GetByID")]
        public HttpResponseMessage GetByID(int id)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Document id");

            var result = Document.SelectByID(id);
            if (result == null || result.CompanyID != CompanyID.Value)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Document could not be found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            
        }

        [HttpPost]
        [ActionName("DefaultAction")]
        public async Task<HttpResponseMessage> PostWithFile()
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = HttpContext.Current.Server.MapPath("~/App_Data/Temp/FileUploads");
            Directory.CreateDirectory(root);
            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            if (result.FormData["model"] == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var model = result.FormData["model"];
            Document doc = JsonConvert.DeserializeObject<Document>(model);
            doc.CompanyID = CompanyID.Value;
            var response = Post(doc);
            if (response.StatusCode != HttpStatusCode.Created) return response;

            if (result.FormData["newfile"] == "true")
            {
                foreach (var file in result.FileData)
                {
                    string url;
                    if (TryMoveUpload(file, "/Uploads/" + doc.OwnerType + "/Documents", false, out url))
                    {
                        DeleteOldDownload(doc.DocumentURL);
                        doc.DocumentURL = url;
                        doc.Update();
                    }
                }
            }
            return Request.CreateResponse<Document>(HttpStatusCode.Created, doc);
        }


        [HttpPut]
        [ActionName("DefaultAction")]
        public async Task<HttpResponseMessage> PutWithFile()
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = HttpContext.Current.Server.MapPath("~/App_Data/Temp/FileUploads");
            Directory.CreateDirectory(root);
            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            if (result.FormData["model"] == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var model = result.FormData["model"];
            Document doc = JsonConvert.DeserializeObject<Document>(model);
            doc.CompanyID = CompanyID.Value;
            var response = Put(doc);
            if (response.StatusCode != HttpStatusCode.OK) return response;
            //get the files
            if (result.FormData["newfile"] == "true")
            {
                //get the files
                foreach (var file in result.FileData)
                {
                    string localRoot = HttpRuntime.AppDomainAppPath;
                    string webRoot = @"/";
                    string uploadFolder = "Uploads";
                    string subFolder = doc.OwnerType;
                    string subSubFolder = doc.OwnerID.ToString();
                    string tempFilename = file.LocalFileName;
                    string type = file.Headers.ContentDisposition.Name.TrimEnd('\"').TrimStart('\"');
                    var period = file.Headers.ContentDisposition.FileName.LastIndexOf('.');
                    string newFilename = doc.ID + "-" + type + file.Headers.ContentDisposition.FileName.Substring(period).TrimEnd('\"');
                    var fullUploadFolder = localRoot + uploadFolder + @"\" + subFolder + @"\" + subSubFolder;
                    var oldFileName = localRoot + doc.DocumentURL;
                    var uniqueFileName = Guid.NewGuid();
                    var newUploadFile = localRoot + uploadFolder + @"\" + subFolder + @"\" + subSubFolder + @"\" + uniqueFileName + file.Headers.ContentDisposition.FileName.Substring(period).TrimEnd('\"');
                    var webPath = webRoot + uploadFolder + @"/" + subFolder + @"/" + subSubFolder + @"/" + uniqueFileName + file.Headers.ContentDisposition.FileName.Substring(period).TrimEnd('\"');
                    try
                    {
                        Directory.CreateDirectory(fullUploadFolder);
                        if (File.Exists(oldFileName)) File.Delete(oldFileName);
                        Directory.Move(tempFilename, newUploadFile);
                        doc.DocumentURL = webPath;
                        doc.Update();
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                        SystemLog.LogNewError(exc, LogType.FileDirectoryError, tempFilename);
                    }
                }
            }
            return Request.CreateResponse<Document>(HttpStatusCode.OK, doc);
        }


        [HttpPost]
        public HttpResponseMessage Post([FromBody]Document value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Document object was not supplied.");

            if (value.ID > 0) return Request.CreateResponse(HttpStatusCode.BadRequest, "Document object already has ID, check Document is not already saved.");

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
        public HttpResponseMessage Put([FromBody]Document value)
        {
            if (!CompanyID.HasValue) return Request.CreateResponse(HttpStatusCode.Unauthorized, "Could not get CompanyID from User");

            if (value == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "Document object was not supplied.");
            
            var result = Document.SelectByID(value.ID);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Document could not be found.");

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

            if (id < 1) return Request.CreateResponse(HttpStatusCode.BadRequest, "Please supply a valid Document id");

            var result = Document.SelectByID(id);

            if (result == null || result.CompanyID != CompanyID.Value) return Request.CreateResponse(HttpStatusCode.NotFound, "Document could not be found.");

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