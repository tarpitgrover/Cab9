using Cab9.Model;
using Cab9.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Cab9.Controller.Common
{
    public abstract class Cab9ApiController : ApiController
    {
        private int? _companyid;
        public int? CompanyID
        {
            get
            {
                if (!_companyid.HasValue)
                {
                    var user = HttpContext.Current.User.Identity as Identity;
                    _companyid = (user != null) ? (int?)user.CompanyID : null;
                }
                return _companyid;
            }
        }

        private int? _userId;
        public int? UserID
        {
            get
            {
                if (!_userId.HasValue)
                {
                    var user = HttpContext.Current.User.Identity as Identity;
                    _userId = (user != null) ? (int?)user.UserObj.ID : null;
                }
                return _userId;
            }
        }
        [HttpOptions]
        [AcceptVerbs("OPTIONS")]
        [ActionName("DefaultAction")]
        public HttpResponseMessage Options()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            return resp;
        }

        protected bool TryMoveUpload(MultipartFileData file, string pathTo, bool resizeImage, out string url)
        {
            url = "";
            try
            {
                string path = HttpRuntime.AppDomainAppPath + pathTo;
                string fileName = Guid.NewGuid() + file.Headers.ContentDisposition.FileName.Substring(file.Headers.ContentDisposition.FileName.LastIndexOf('.')).TrimEnd('\"');
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (resizeImage)
                {
                    using (Image image = Image.FromFile(file.LocalFileName))
                    {
                        var result = image.TakeSquare(500, 500);
                        result.Save(path + '/' + fileName);
                    }
                }
                else
                {
                    Directory.Move(file.LocalFileName, path + '/' + fileName);
                }
                url = pathTo + '/' + fileName;
            }
            catch (Exception exc)
            {
                return false;
            }
            return true;
        }

        protected void DeleteOldDownload(string webPath)
        {
            if (webPath != null && File.Exists(HttpRuntime.AppDomainAppPath + webPath))
                File.Delete(HttpRuntime.AppDomainAppPath + webPath);
        }
    }
}