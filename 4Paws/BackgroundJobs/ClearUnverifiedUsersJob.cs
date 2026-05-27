using _4Paws.Data;

namespace _4Paws.BackgroundJobs
{
    public class ClearUnverifiedUsersJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ClearUnverifiedUsersJob> _logger;

        // Runs every 24 hours
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);

        public ClearUnverifiedUsersJob(IServiceScopeFactory scopeFactory, ILogger<ClearUnverifiedUsersJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ClearUnverifiedUsersJob started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ClearUnverifiedUsers();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ClearUnverifiedUsersJob encountered an error.");
                }

                // Wait 24 hours before running again
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("ClearUnverifiedUsersJob stopped.");
        }

        private async Task ClearUnverifiedUsers()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            // Only delete users who registered more than 24 hours ago and never verified
            // This gives users a full day to check their email
            var cutoff = DateTime.UtcNow.AddHours(-24);

            var unverified = db.Users
                .Where(u => !u.IsVerified && u.CreatedAt < cutoff)
                .ToList();

            if (!unverified.Any())
            {
                _logger.LogInformation("ClearUnverifiedUsersJob: No unverified users to clear.");
                return;
            }

            // Soft delete — SaveChanges() intercepts and sets IsDeleted=true
            db.Users.RemoveRange(unverified);
            await db.SaveChangesAsync();

            _logger.LogInformation(
                "ClearUnverifiedUsersJob: Soft-deleted {Count} unverified user(s).", unverified.Count);
        }
    }
}
