
using _4Paws.Data;
using _4Paws.Extensions;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
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

namespace _4Paws
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddServices(builder.Configuration);

            var app = builder.Build();

            app.UseApp();
            app.Run();

        }
    }
}