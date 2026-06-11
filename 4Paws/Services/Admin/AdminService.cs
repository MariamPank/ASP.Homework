using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Admin.Responses;
using _4Paws.DTOs.Agreement.Responses;
using _4Paws.DTOs.Application.Responses;
using _4Paws.DTOs.Listing.Responses;
using _4Paws.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace _4Paws.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly DataContext _db;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        private const string STATS_KEY = "admin_stats";
        private readonly TimeSpan CACHE_TTL = TimeSpan.FromMinutes(5);

        public AdminService(DataContext db, IMapper mapper, IMemoryCache cache)
        {
            _db = db;
            _mapper = mapper;
            _cache = cache;
        }

        // ── Users ─────────────────────────────────────────────────────────

        public Result<IEnumerable<AdminUserResponse>> GetAllUsers()
        {
            var users = _db.Users
                .Include(u => u.Owner)
                .Include(u => u.CareGiver)
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return Result<IEnumerable<AdminUserResponse>>.Ok(_mapper.Map<IEnumerable<AdminUserResponse>>(users));
        }

        public Result<IEnumerable<AdminUserResponse>> GetDeletedUsers()
        {
            var users = _db.Users
                .IgnoreQueryFilters()
                .Include(u => u.Owner)
                .Include(u => u.CareGiver)
                .Where(u => u.IsDeleted)
                .OrderByDescending(u => u.DeletedAt)
                .ToList();

            return Result<IEnumerable<AdminUserResponse>>.Ok(_mapper.Map<IEnumerable<AdminUserResponse>>(users));
        }

        public Result<AdminUserResponse> GetUserById(int userId)
        {
            var user = _db.Users
                .Include(u => u.Owner)
                .Include(u => u.CareGiver)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Result<AdminUserResponse>.NotFound("User not found.");

            return Result<AdminUserResponse>.Ok(_mapper.Map<AdminUserResponse>(user));
        }

        public Result<int> DeleteUser(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Result<int>.NotFound("User not found.");
            if (user.Role == UserRole.Admin) return Result<int>.BadRequest("Cannot delete another admin.");

            _db.Users.Remove(user);
            _db.SaveChanges();

            // Invalidate stats — user count changed
            _cache.Remove(STATS_KEY);

            return Result<int>.Ok(userId);
        }

        public Result<int> RestoreUser(int userId)
        {
            var user = _db.Users
                .IgnoreQueryFilters()
                .FirstOrDefault(u => u.Id == userId && u.IsDeleted);

            if (user == null) return Result<int>.NotFound("Deleted user not found.");

            user.IsDeleted = false;
            user.DeletedAt = null;
            _db.SaveChanges();

            _cache.Remove(STATS_KEY);

            return Result<int>.Ok(userId);
        }

        public Result<int> BanUser(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Result<int>.NotFound("User not found.");
            if (user.Role == UserRole.Admin) return Result<int>.BadRequest("Cannot ban another admin.");
            if (user.IsBanned) return Result<int>.BadRequest("User is already banned.");

            user.IsBanned = true;
            _db.SaveChanges();

            _cache.Remove(STATS_KEY);

            return Result<int>.Ok(userId);
        }

        public Result<int> UnbanUser(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Result<int>.NotFound("User not found.");
            if (!user.IsBanned) return Result<int>.BadRequest("User is not banned.");

            user.IsBanned = false;
            _db.SaveChanges();

            _cache.Remove(STATS_KEY);

            return Result<int>.Ok(userId);
        }

        public Result<int> ClearUnverifiedUsers()
        {
            try
            {
                var unverifiedUsers = _db.Users.Where(u => !u.IsVerified).ToList();
                int count = unverifiedUsers.Count;

                _db.Users.RemoveRange(unverifiedUsers);
                _db.SaveChanges();

                _cache.Remove(STATS_KEY);

                return Result<int>.Ok(count);
            }
            catch (Exception ex)
            {
                return Result<int>.BadRequest("Could not clear users: " + ex.Message);
            }
        }

        // ── Listings ──────────────────────────────────────────────────────

        public Result<IEnumerable<ListingResponse>> GetAllListings()
        {
            var listings = _db.Listings
                .Include(l => l.Pet)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new ListingResponse
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    ListingType = l.ListingType,
                    Status = l.Status,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    CreatedAt = l.CreatedAt,
                    ProposedBudget = l.ProposedBudget,
                    PetName = l.Pet != null ? l.Pet.PetName : l.PetName,
                    OwnerId = l.OwnerId,
                    CareGiverId = l.CareGiverId,
                    PetId = l.PetId,
                })
                .ToList();

            return Result<IEnumerable<ListingResponse>>.Ok(listings);
        }

        public Result<IEnumerable<ListingResponse>> GetDeletedListings()
        {
            var listings = _db.Listings
                .IgnoreQueryFilters()
                .Include(l => l.Pet)
                .Where(l => l.IsDeleted)
                .OrderByDescending(l => l.DeletedAt)
                .Select(l => new ListingResponse
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    ListingType = l.ListingType,
                    Status = l.Status,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    CreatedAt = l.CreatedAt,
                    ProposedBudget = l.ProposedBudget,
                    PetName = l.Pet != null ? l.Pet.PetName : l.PetName,
                    OwnerId = l.OwnerId,
                    CareGiverId = l.CareGiverId,
                    PetId = l.PetId,
                })
                .ToList();

            return Result<IEnumerable<ListingResponse>>.Ok(listings);
        }

        public Result<int> DeleteListing(int listingId)
        {
            var listing = _db.Listings.FirstOrDefault(l => l.Id == listingId);
            if (listing == null) return Result<int>.NotFound("Listing not found.");

            _db.Listings.Remove(listing);
            _db.SaveChanges();

            _cache.Remove(STATS_KEY);

            return Result<int>.Ok(listingId);
        }

        public Result<int> RestoreListing(int listingId)
        {
            var listing = _db.Listings
                .IgnoreQueryFilters()
                .FirstOrDefault(l => l.Id == listingId && l.IsDeleted);

            if (listing == null) return Result<int>.NotFound("Deleted listing not found.");

            listing.IsDeleted = false;
            listing.DeletedAt = null;
            _db.SaveChanges();

            _cache.Remove(STATS_KEY);

            return Result<int>.Ok(listingId);
        }

        // ── Applications ──────────────────────────────────────────────────

        public Result<IEnumerable<ApplicationResponse>> GetAllApplications()
        {
            var applications = _db.Applications
                .Include(a => a.Owner).ThenInclude(o => o.User)
                .Include(a => a.CareGiver).ThenInclude(c => c.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToList()
                .Select(a => new ApplicationResponse
                {
                    Id = a.Id,
                    ListingId = a.ListingId,
                    ApplicantId = a.OwnerId ?? a.CareGiverId ?? 0,
                    ApplicantName = a.Owner?.User?.FullName
                        ?? a.CareGiver?.User?.FullName
                        ?? "Unknown",
                    Message = a.Message,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt,
                });

            return Result<IEnumerable<ApplicationResponse>>.Ok(applications);
        }

        // ── Agreements ────────────────────────────────────────────────────

        public Result<IEnumerable<AgreementResponse>> GetAllAgreements()
        {
            var agreements = _db.Agreements
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AgreementResponse
                {
                    Id = a.Id,
                    Status = a.Status,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    AgreedFee = a.AgreedFee,
                    OwnerId = a.OwnerId,
                    CareGiverId = a.CareGiverId,
                    PetId = a.PetId,
                    CompleteAt = a.CompleteAt,
                })
                .ToList();

            return Result<IEnumerable<AgreementResponse>>.Ok(agreements);
        }

        // ── Stats — cached ────────────────────────────────────────────────

        public Result<AdminStatsResponse> GetStats()
        {
            if (_cache.TryGetValue(STATS_KEY, out AdminStatsResponse cached))
                return Result<AdminStatsResponse>.Ok(cached);

            var stats = new AdminStatsResponse
            {
                TotalUsers = _db.Users.Count(),
                TotalOwners = _db.Owners.Count(),
                TotalCareGivers = _db.CareGivers.Count(),
                TotalPets = _db.Pets.Count(),
                TotalListings = _db.Listings.Count(),
                ActiveListings = _db.Listings.Count(l => l.Status == ListingStatus.Open),
                TotalApplications = _db.Applications.Count(),
                TotalAgreements = _db.Agreements.Count(),
                ActiveAgreements = _db.Agreements.Count(a => a.Status == AgreementStatus.Active),
                CompletedAgreements = _db.Agreements.Count(a => a.Status == AgreementStatus.Completed),
                BannedUsers = _db.Users.Count(u => u.IsBanned),
                DeletedUsers = _db.Users.IgnoreQueryFilters().Count(u => u.IsDeleted),
                DeletedListings = _db.Listings.IgnoreQueryFilters().Count(l => l.IsDeleted),
            };

            _cache.Set(STATS_KEY, stats, CACHE_TTL);

            return Result<AdminStatsResponse>.Ok(stats);
        }
    }
}
