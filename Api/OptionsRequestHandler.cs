using Cab9.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Cab9.Handlers
{
    public class OptionsRequestHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Options)
            {
                var Response = new HttpResponseMessage(HttpStatusCode.OK);
                IEnumerable<string> values;
                if (request.Headers.TryGetValues("Origin", out values))
                    Response.Headers.Add("Access-Control-Allow-Origin", values);
                else
                    Response.Headers.Add("Access-Control-Allow-Origin", "*");
                Response.Headers.Add("AcceptEncoding", "gzip, deflate");
                Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE");
                Response.Headers.Add("Access-Control-Max-Age", "10000");
                Response.Headers.Add("Access-Control-Allow-Headers", "X-Requested-With, Content-Type, ApiUserKey, Authorization");
                return Task.Factory.StartNew(() => { return Response; });
            }

            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;
                return response;
            });
        }
    }
}