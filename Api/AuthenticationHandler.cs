using Cab9.Model;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Cab9.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var rawAuthCookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);

            if (rawAuthCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(rawAuthCookie.Value);
                User user = User.SelectByEmail(ticket.Name);
                Identity ident = new Identity(user);
                Principal princ = new Principal(ident);   
        
                HttpContext.Current.User = princ;
                FormsAuthentication.RenewTicketIfOld(ticket);
            }
#if DEBUG
            else
            {
                User user = new User() { CompanyID = 1 };
                Identity ident = new Identity(user);
                Principal princ = new Principal(ident);
                HttpContext.Current.User = princ;
            }
#endif

            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var response = task.Result;
                return response;
            });
        }
    }
}