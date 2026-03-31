
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using ReminderEmail.Data;
using ReminderEmail.Services;

namespace ReminderEmail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<GmailEmailService>();
            builder.Services.AddScoped<ReminderService>();

            builder.Services.AddHangfire(config => config.UseMemoryStorage());

            builder.Services.AddHangfireServer();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<ReminderService>(
                "send-login-reminders",
                x => x.SendReminders(),
                Cron.Minutely);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}