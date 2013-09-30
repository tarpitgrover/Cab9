using Cab9.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Cab9.Model
{
    public class Identity : IIdentity
    {
        public string[] Roles { get; set; }
        public int CompanyID { get; set; }
        public string AuthType { get; set; }
        private bool _isAuthed { get; set; }
        private string _username { get; set; }

        private User _user { get; set; }
        public User UserObj
        {
            get { if (_user == null) _user = User.SelectByEmail(Name); return _user; }
            set { _user = value; }
        }

        public Identity(User user)
        {
            UserObj = user;
            _username = user.Email;
            Roles = user.Roles;
            CompanyID = user.CompanyID;
            _isAuthed = true;
            AuthType = "Cab9";
        }

        public string AuthenticationType
        {
            get { return AuthType; }
        }

        public bool IsAuthenticated
        {
            get { return _isAuthed; }
        }

        public string Name
        {
            get { return _username; }
        }
    }
}