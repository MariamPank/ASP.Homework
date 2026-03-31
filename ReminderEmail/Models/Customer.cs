using ReminderEmail.Common.Entity;

namespace ReminderEmail.Models
{
    public class Customer : Entity
    {
        public string CustomerName { get; set; }
        public DateTime LogInDate {  get; set; }
        public string Email { get; set; }
        public bool IsReminderSent { get; set; }

        public List<EmailLog> EmailLogs { get; set; } = new();
    }
}
