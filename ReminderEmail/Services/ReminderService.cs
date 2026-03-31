using ReminderEmail.Data;
using ReminderEmail.Models;

namespace ReminderEmail.Services
{
    public class ReminderService
    {
        private readonly DataContext _db;
        private readonly GmailEmailService _gmailEmailService;

        public ReminderService(DataContext db, GmailEmailService gmailEmailService)
        {
            _db = db;
            _gmailEmailService = gmailEmailService;
        }

        public List<Customer> SleepyCustomers()
        {
            var inactivePeriod = DateTime.Now.AddDays(-7);

            var sleepyCustomers = _db.Customers
                .Where(e => e.LogInDate < inactivePeriod && e.IsReminderSent == false)
                .ToList();

            return sleepyCustomers;
        }

        public void SendReminders()
        {
            var sleepyCustomers = SleepyCustomers();

            foreach (var customer in sleepyCustomers)
            {
                if (customer.Id != 3) continue;

                string subject = "We miss you!";
                string body = $"Hello {customer.CustomerName}, you have not logged in for 7 days. Please log in again.";

                _gmailEmailService.SendEmail(subject, customer.Email, body);

                var emailLog = new EmailLog
                {
                    Subject = subject,
                    Body = body,
                    ReceiverEmail = customer.Email,
                    SentAt = DateTime.Now,
                    CustomerId = customer.Id,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _db.EmailLogs.Add(emailLog);

                customer.IsReminderSent = true;
                customer.UpdatedDate = DateTime.Now;
            }

            _db.SaveChanges();
        }
    }
}
