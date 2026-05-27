using _4Paws.Data;
using _4Paws.Enums;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.BackgroundJobs
{
    public class ExpireListingsJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpireListingsJob> _logger;

        // Runs every 1 hour
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public ExpireListingsJob(IServiceScopeFactory scopeFactory, ILogger<ExpireListingsJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExpireListingsJob started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExpireListings();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExpireListingsJob encountered an error.");
                }

                // Wait 1 hour before running again
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("ExpireListingsJob stopped.");
        }

        private async Task ExpireListings()
        {
            // BackgroundService is a singleton — must create a scope to use scoped DbContext
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            var expired = await db.Listings
                .Where(l => l.Status == ListingStatus.Open
                         && l.EndDate < DateTime.UtcNow)
                .ToListAsync();

            if (!expired.Any())
            {
                _logger.LogInformation("ExpireListingsJob: No expired listings found.");
                return;
            }

            foreach (var listing in expired)
                listing.Status = ListingStatus.Closed;

            await db.SaveChangesAsync();

            _logger.LogInformation(
                "ExpireListingsJob: Closed {Count} expired listing(s).", expired.Count);
        }
    }
}

