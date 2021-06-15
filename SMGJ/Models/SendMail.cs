using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SMGJ.Models
{
    public class SendMail
    {
        public static void DergoEmail(string per, string subjekti, string body)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["Email"]);
                message.To.Add(per);
                message.Subject = subjekti;
                message.Body = body;
                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["DomainServer"]);
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"], ConfigurationManager.AppSettings["Password"]);
                smtp.EnableSsl = true;
                try
                {
                    smtp.Send(message);
                }
                catch (SmtpFailedRecipientException ex)
                {
                    ex.GetBaseException();
                    // should give you enough info.
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}