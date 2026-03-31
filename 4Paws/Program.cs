
using _4Paws.Data;
using _4Paws.Helper.Services;
using _4Paws.Services.Auth;
using _4Paws.Services.CareGiver;
using _4Paws.Services.Owner;
using _4Paws.Services.Pet;
using _4Paws.Services.User;
using _4Paws.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace _4Paws
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IOwnerService, OwnerService>();
            builder.Services.AddScoped<ICaregiverService, CaregiverService>();
            builder.Services.AddScoped<IPetService, PetService>();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ?? ??? Authentication ?? ?????, ?? ??????? ????????
            // app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}