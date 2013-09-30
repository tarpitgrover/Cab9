using Cab9.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Cab9.Model
{
    public class Principal : IPrincipal
    {
        private Identity _identity { get; set; }
        private string[] _roles { get; set; }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            var rp = new Cab9RoleProvider();
            return rp.IsUserInRole(_identity.Name, role);
        }

        public Principal(Identity ident)
        {
            _identity = ident;
            _roles = ident.Roles;
        }
    }
}
