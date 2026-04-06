using System.Net;
using System.Net.Mail;

namespace _4Paws.Common.Services
{
    public class SmtpService
    {
        private string _email = "testerhelper47@gmail.com";
        private string _password = "ofkb hfxl fbhb tayc";

        public void SendEmail(string subject, string email, string body)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(_email, "4Pets");
            mail.Subject = subject;
            mail.Body = body;
            mail.To.Add(email);
            mail.IsBodyHtml = false;

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(_email, _password)
            };

            smtp.Send(mail);
        }
    }
}
