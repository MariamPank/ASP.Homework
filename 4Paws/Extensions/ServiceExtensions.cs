using _4Paws.Common.Services;
using _4Paws.Data;
using _4Paws.Helper.Adm;
using _4Paws.Helper.CareGiver;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using _4Paws.Services.Admin;
using _4Paws.Services.Agreement;
using _4Paws.Services.Application;
using _4Paws.Services.Auth;
using _4Paws.Services.CareGiver;
using _4Paws.Services.Listing;
using _4Paws.Services.Owner;
using _4Paws.Services.Pet;
using _4Paws.Services.User;
using _4Paws.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                            origin.StartsWith("http://localhost") ||
                            origin.StartsWith("https://localhost"))
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            services.AddJwt(config);

            services.AddDbContext<DataContext>(o => o.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddHttpContextAccessor();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAgreementService, AgreementService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ICaregiverService, CaregiverService>();
            services.AddScoped<IListingService, ListingService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentOwner, CurrentOwner>();
            services.AddScoped<ICurrentCareGiver, CurrentCareGiver>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAdministrator, Administrator>();

            services.AddScoped<SmtpService>();
            services.AddScoped<JwtService>();
            return services;
        }
    }
}
