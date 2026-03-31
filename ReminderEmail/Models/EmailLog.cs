using ReminderEmail.Common.Entity;

namespace ReminderEmail.Models
{
    public class EmailLog : Entity
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ReceiverEmail { get; set; }
        public DateTime SentAt { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
