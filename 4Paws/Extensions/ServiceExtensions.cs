using _4Paws.BackgroundJobs;
using _4Paws.Common.Services;
using _4Paws.Data;
using _4Paws.Helper.Adm;
using _4Paws.Helper.CareGiver;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using _4Paws.Profiles;
using _4Paws.Services.Admin;
using _4Paws.Services.Agreement;
using _4Paws.Services.Application;
using _4Paws.Services.Auth;
using _4Paws.Services.CareGiver;
using _4Paws.Services.Images;
using _4Paws.Services.Listing;
using _4Paws.Services.Owner;
using _4Paws.Services.Pet;
using _4Paws.Services.Review;
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

            services.AddDbContext<DataContext>(o => o.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                )
            ));

            services.AddJwt(config);

            // ── Second Level Cache ────────────────────────────────────────
            // Built-in in-memory cache — no extra packages needed.
            // Caches: all open listings, owner/caregiver profiles,
            //         dashboards, and admin stats (5 min TTL each).
            // Mention Redis as future upgrade for horizontal scaling.
            services.AddMemoryCache();

            services.AddHttpContextAccessor();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAgreementService, AgreementService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ICaregiverService, CaregiverService>();
            services.AddScoped<IListingService, ListingService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentOwner, CurrentOwner>();
            services.AddScoped<ICurrentCareGiver, CurrentCareGiver>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAdministrator, Administrator>();
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddSingleton(new CloudinaryService());

            services.AddScoped<SmtpService>();
            services.AddScoped<JwtService>();

            // ── File Upload ───────────────────────────────────────────────
            services.AddScoped<FileUploadService>();

            // ── Background Jobs ───────────────────────────────────────────
            services.AddHostedService<ExpireListingsJob>();
            services.AddHostedService<ClearUnverifiedUsersJob>();

            return services;
        }
    }
}
