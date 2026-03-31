using ReminderEmail.Models;
using System.Net;
using System.Net.Mail;

namespace ReminderEmail.Services
{
    public class GmailEmailService
    {
        private string _email = "testerhelper44@gmail.com";
        private string _password = "eiws uhcl rmyp lhek"; // Gmail App Password

        public void SendEmail(string subject, string toEmail, string body)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(_email, "Reminder to Log in");
            mail.Subject = subject;
            mail.Body = body;
            mail.To.Add(toEmail);
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
