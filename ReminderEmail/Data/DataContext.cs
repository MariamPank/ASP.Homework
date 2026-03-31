using Microsoft.EntityFrameworkCore;
using ReminderEmail.Models;

namespace ReminderEmail.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
        }
    }
}
