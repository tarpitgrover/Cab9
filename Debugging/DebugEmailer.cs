using System;
using System.Globalization;
using System.Net.Mail;

namespace e9.Debugging
{
    public class DebugEmailer
    {
        public static void Email(string body)
        {
            //var mail = new MailAddress("dbeech@e9ine.com");
            //var message = new MailMessage(mail, mail)
            //{
            //    IsBodyHtml = true,
            //    Subject = "Debugging",
            //    Priority = MailPriority.Normal
            //};
            //message.Body = body + "<br />" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            //var client = new SmtpClient();
            //client.Send(message);
        }

        public static void Email(Exception exc)
        {
            //var mail = new MailAddress("dbeech@e9ine.com");
            //var message = new MailMessage(mail, mail)
            //{
            //    IsBodyHtml = true,
            //    Subject = "Debugging",
            //    Priority = MailPriority.Normal
            //};
            //message.Body = exc.Message + "<br />" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            //var client = new SmtpClient();
            //client.Send(message);
        }
    }
}