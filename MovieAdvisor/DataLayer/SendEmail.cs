using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MovieAdvisor.DataLayer
{
    public static class SendEmail
    {
        public static void Email(string messageBody,string fromEmail,string pass,string toEmail,string Subject)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(fromEmail);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = Subject;
                message.Body = messageBody;
                smtp.Port = 587;
                if (fromEmail.Contains("gmail"))
                {
                    smtp.Host = "smtp.gmail.com"; //for gmail host  
                }
                else if (fromEmail.Contains("hotmail"))
                {
                    smtp.Host = "smtp.office365.com"; //for hotmail host  
                }
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(fromEmail, pass);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
