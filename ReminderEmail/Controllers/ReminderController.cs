using Microsoft.AspNetCore.Mvc;
using ReminderEmail.Data;
using ReminderEmail.Models;
using ReminderEmail.Services;

namespace ReminderEmail.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderService _reminderService;
        private readonly DataContext _db;

    public ReminderController(ReminderService reminderService, DataContext db)
        {
            _reminderService = reminderService;
            _db = db;
        }

        // 1. ხელით გაუშვი reminder (Hangfire-ის გარეშე ტესტისთვის)
        [HttpPost("run")]
        public IActionResult Run()
        {
            _reminderService.SendReminders();
            return Ok("Reminders sent successfully");
        }

        // 2. შექმენი სატესტო inactive customer
        [HttpPost("test-customer")]
        public IActionResult CreateTestCustomer()
        {
            var customer = new Customer
            {
                CustomerName = "Test User",
                Email = "mari.pankelashvili@gmail.com", // შეცვალე შენი email-ით
                LogInDate = DateTime.Now.AddDays(-10),
                IsReminderSent = false,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _db.Customers.Add(customer);
            _db.SaveChanges();

            return Ok(customer);
        }

        // 3. fake login (reset reminder-ისთვის)
        [HttpPost("login/{id}")]
        public IActionResult Login(int id)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.Id == id);

            if (customer == null)
                return NotFound("Customer not found");

            customer.LogInDate = DateTime.Now;
            customer.IsReminderSent = false;
            customer.UpdatedDate = DateTime.Now;

            _db.SaveChanges();

            return Ok("Login updated successfully");
        }

        // 4. ყველა customer-ის ნახვა (debug-ისთვის)
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var customers = _db.Customers.ToList();
            return Ok(customers);
        }

        // 5. Email logs-ის ნახვა
        [HttpGet("logs")]
        public IActionResult GetLogs()
        {
            var logs = _db.EmailLogs.ToList();
            return Ok(logs);
        }
    }

}

