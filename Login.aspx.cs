using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cab9
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            var u = Username.Text;
            var p = Password.Text;
            string r = "";

            var user = Cab9.Model.User.ValidateUser(u, p, out r);
            if (user != null)
            {
                FormsAuthentication.RedirectFromLoginPage(user.Email, false);
            }

        }
    }
}