using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Owner.Requests;
using _4Paws.DTOs.Owner.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using Microsoft.Extensions.Caching.Memory;

namespace _4Paws.Services.Owner
{
    public class OwnerService : IOwnerService
    {
        private readonly DataContext _db;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentOwner _currentOwner;
        private readonly IMemoryCache _cache;

        private readonly TimeSpan CACHE_TTL = TimeSpan.FromMinutes(5);

        public OwnerService(
            DataContext db,
            ICurrentUserService currentUser,
            ICurrentOwner currentOwner,
            IMemoryCache cache)
        {
            _db = db;
            _currentUser = currentUser;
            _currentOwner = currentOwner;
            _cache = cache;
        }

        public Result<CreateOwnerProfileResponse> CreateOwnerProfile(CreateOwnerProfileRequest request)
        {
            if (request == null)
                return Result<CreateOwnerProfileResponse>.BadRequest("Request is null");

            var userId = _currentUser.CurrentUserId();

            var userExists = _db.Users.Any(x => x.Id == userId);
            if (!userExists)
                return Result<CreateOwnerProfileResponse>.NotFound("User not found");

            var ownerExists = _db.Owners.Any(x => x.UserId == userId);
            if (ownerExists)
                return Result<CreateOwnerProfileResponse>.BadRequest("Owner profile already exists");

            var owner = new Models.Owner
            {
                UserName = request.UserName,
                OwnerRating = Rating.Average,
                UserId = userId
            };

            _db.Owners.Add(owner);
            _db.SaveChanges();

            return Result<CreateOwnerProfileResponse>.Ok(new CreateOwnerProfileResponse
            {
                Id = owner.Id,
                UserName = owner.UserName,
                OwnerRating = owner.OwnerRating,
                UserId = owner.UserId
            });
        }

        public Result<GetOwnerByIdResponse> GetOwnerById(int ownerId)
        {
            // ── Cache key scoped to ownerId ───────────────────────────────
            var cacheKey = $"owner_profile_{ownerId}";

            if (_cache.TryGetValue(cacheKey, out GetOwnerByIdResponse cached))
                return Result<GetOwnerByIdResponse>.Ok(cached);

            var ownerExists = _db.Owners.Any(x => x.Id == ownerId);
            if (!ownerExists)
                return Result<GetOwnerByIdResponse>.NotFound("Owner not found");

            var owner = _db.Owners
                .Where(x => x.Id == ownerId)
                .Select(x => new GetOwnerByIdResponse
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    OwnerRating = x.OwnerRating,
                    UserId = x.UserId
                })
                .FirstOrDefault();

            _cache.Set(cacheKey, owner, CACHE_TTL);

            return Result<GetOwnerByIdResponse>.Ok(owner);
        }

        public Result<GetOwnerDashboardResponse> GetOwnerDashboard(int ownerId)
        {
            // ── Cache key scoped to ownerId ───────────────────────────────
            var cacheKey = $"owner_dashboard_{ownerId}";

            if (_cache.TryGetValue(cacheKey, out GetOwnerDashboardResponse cached))
                return Result<GetOwnerDashboardResponse>.Ok(cached);

            var ownerExists = _db.Owners.Any(x => x.Id == ownerId);
            if (!ownerExists)
                return Result<GetOwnerDashboardResponse>.NotFound("Owner not found");

            var dashboard = _db.Owners
                .Where(x => x.Id == ownerId)
                .Select(x => new GetOwnerDashboardResponse
                {
                    OwnerId = x.Id,
                    UserName = x.UserName,
                    OwnerRating = x.OwnerRating,

                    TotalPets = x.Pets.Count(),
                    TotalListings = x.Listings.Count(),
                    ActiveListings = x.Listings.Count(l => l.Status == ListingStatus.Open),
                    TotalAgreements = x.Agreements.Count(),
                    ActiveAgreements = x.Agreements.Count(a => a.Status == AgreementStatus.Active),
                    CompletedAgreements = x.Agreements.Count(a => a.Status == AgreementStatus.Completed),

                    RecentListings = x.Listings
                        .OrderByDescending(l => l.Id).Take(5)
                        .Select(l => new OwnerListingShortResponse
                        {
                            Id = l.Id,
                            Title = l.Title,
                            Status = l.Status
                        }).ToList(),

                    RecentAgreements = x.Agreements
                        .OrderByDescending(a => a.Id).Take(5)
                        .Select(a => new OwnerAgreementShortResponse
                        {
                            Id = a.Id,
                            Status = a.Status,
                            PetName = a.Pet.PetName,
                            CareGiverName = a.CareGiver.CareGiverName
                        }).ToList()
                })
                .FirstOrDefault();

            if (dashboard == null)
                return Result<GetOwnerDashboardResponse>.NotFound("Owner dashboard not found");

            _cache.Set(cacheKey, dashboard, CACHE_TTL);

            return Result<GetOwnerDashboardResponse>.Ok(dashboard);
        }

        public Result<List<GetOwnerAgreementsResponse>> GetOwnerAgreements(int ownerId)
        {
            var ownerExists = _db.Owners.Any(x => x.Id == ownerId);
            if (!ownerExists)
                return Result<List<GetOwnerAgreementsResponse>>.NotFound("Owner not found");

            var agreements = _db.Agreements
                .Where(x => x.OwnerId == ownerId)
                .Select(x => new GetOwnerAgreementsResponse
                {
                    Id = x.Id,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    AgreedFee = x.AgreedFee,
                    CareGiverName = x.CareGiver.CareGiverName,
                    PetName = x.Pet.PetName
                })
                .ToList();

            return Result<List<GetOwnerAgreementsResponse>>.Ok(agreements);
        }

        public Result<List<GetOwnerListingsResponse>> GetOwnerListings(int ownerId)
        {
            var ownerExists = _db.Owners.Any(x => x.Id == ownerId);
            if (!ownerExists)
                return Result<List<GetOwnerListingsResponse>>.NotFound("Owner not found");

            var listings = _db.Listings
                .Where(x => x.OwnerId == ownerId)
                .Select(x => new GetOwnerListingsResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    ProposedBudget = x.ProposedBudget,
                })
                .ToList();

            return Result<List<GetOwnerListingsResponse>>.Ok(listings);
        }

        // ── Cache invalidation helper — call when owner data changes ──────
        public void InvalidateOwnerCache(int ownerId)
        {
            _cache.Remove($"owner_profile_{ownerId}");
            _cache.Remove($"owner_dashboard_{ownerId}");
        }
    }
}
