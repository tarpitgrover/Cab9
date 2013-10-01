using Cab9.Hubs;
using Cab9.Model;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Cab9
{
    public partial class Test : System.Web.UI.Page
    {
        public static IHubContext Hub { get { return GlobalHost.ConnectionManager.GetHubContext<OfficeHub>(); } }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var booking = Booking.SelectByID(Int64.Parse(TextBox1.Text));
            booking.Push(Int32.Parse(TextBox2.Text));
        }
    }
}