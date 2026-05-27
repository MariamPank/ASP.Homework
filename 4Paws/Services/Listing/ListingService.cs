using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Listing.Requests;
using _4Paws.DTOs.Listing.Responses;
using _4Paws.Enums;
using _4Paws.Helper.CareGiver;
using _4Paws.Helper.Owner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace _4Paws.Services.Listing
{
    public class ListingService : IListingService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;
        private readonly ICurrentCareGiver _currentCareGiver;
        private readonly IMemoryCache _cache;

        // ── Cache keys ────────────────────────────────────────────────────
        private const string ALL_LISTINGS_KEY = "all_open_listings";
        private readonly TimeSpan CACHE_TTL = TimeSpan.FromMinutes(5);

        public ListingService(
            DataContext db,
            ICurrentOwner currentOwner,
            ICurrentCareGiver currentCaregiver,
            IMemoryCache cache)
        {
            _db = db;
            _currentOwner = currentOwner;
            _currentCareGiver = currentCaregiver;
            _cache = cache;
        }

        public Result<ListingResponse> CreateListing(CreateListingRequest request)
        {
            if (request == null)
                return Result<ListingResponse>.BadRequest("Request is null");

            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (request.ListingType == ListingType.OwnerNeedsCareGiver)
            {
                if (owner == null)
                    return Result<ListingResponse>.NotFound("Owner profile not found. You must create an owner profile first.");

                if (!request.PetId.HasValue)
                    return Result<ListingResponse>.BadRequest("PetId is required for owner listings.");

                if (_db.Listings.Any(x => x.OwnerId == owner.Id && x.PetId == request.PetId && x.Status == ListingStatus.Open))
                    return Result<ListingResponse>.BadRequest("An open listing for this pet already exists.");
            }
            else if (request.ListingType == ListingType.CareGiverOffersService)
            {
                if (careGiver == null)
                    return Result<ListingResponse>.NotFound("Caregiver profile not found. You must create a caregiver profile first.");

                if (_db.Listings.Any(x => x.CareGiverId == careGiver.Id && x.ListingType == ListingType.CareGiverOffersService && x.Status == ListingStatus.Open))
                    return Result<ListingResponse>.BadRequest("You already have an active 'Service Offer' listing.");
            }

            var listing = new Models.Listing
            {
                Title = request.Title,
                Description = request.Description,
                ListingType = request.ListingType,
                Status = ListingStatus.Open,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ProposedBudget = request.ProposedBudget,
                OwnerId = (request.ListingType == ListingType.OwnerNeedsCareGiver) ? owner.Id : null,
                PetId = (request.ListingType == ListingType.OwnerNeedsCareGiver) ? request.PetId : null,
                CareGiverId = (request.ListingType == ListingType.CareGiverOffersService) ? careGiver.Id : null
            };

            _db.Listings.Add(listing);
            _db.SaveChanges();

            // ── Invalidate cache — new listing added ──────────────────────
            _cache.Remove(ALL_LISTINGS_KEY);

            return Result<ListingResponse>.Ok(MapToResponse(listing));
        }

        public Result<bool> DeleteListing(int id)
        {
            var listingToDelete = _db.Listings.FirstOrDefault(x => x.Id == id);

            if (listingToDelete == null)
                return Result<bool>.NotFound("Listing not found.");

            if (listingToDelete.ListingType == ListingType.OwnerNeedsCareGiver)
            {
                var owner = _currentOwner.GetCurrentOwner();
                if (owner == null || listingToDelete.OwnerId != owner.Id)
                    return Result<bool>.BadRequest("You do not have permission to delete this owner listing.");
            }
            else if (listingToDelete.ListingType == ListingType.CareGiverOffersService)
            {
                var careGiver = _currentCareGiver.GetCurrentCareGiver();
                if (careGiver == null || listingToDelete.CareGiverId != careGiver.Id)
                    return Result<bool>.BadRequest("You do not have permission to delete this service offer.");
            }

            _db.Listings.Remove(listingToDelete);
            _db.SaveChanges();

            // ── Invalidate cache — listing removed ────────────────────────
            _cache.Remove(ALL_LISTINGS_KEY);

            return Result<bool>.Ok(true);
        }

        public Result<IEnumerable<ListingResponse>> GetAllActiveListings()
        {
            // ── Try cache first ───────────────────────────────────────────
            if (_cache.TryGetValue(ALL_LISTINGS_KEY, out IEnumerable<ListingResponse> cached))
                return Result<IEnumerable<ListingResponse>>.Ok(cached);

            // ── Cache miss — query DB ─────────────────────────────────────
            var listings = _db.Listings
                .Include(x => x.Pet)
                .Where(x => x.Status == ListingStatus.Open)
                .ToList()
                .Select(MapToResponse);

            _cache.Set(ALL_LISTINGS_KEY, listings, CACHE_TTL);

            return Result<IEnumerable<ListingResponse>>.Ok(listings);
        }

        public Result<ListingResponse> GetListingById(int id)
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            var listing = _db.Listings
                .Include(x => x.Pet)
                .FirstOrDefault(x => x.Id == id);

            if (listing == null)
                return Result<ListingResponse>.NotFound("Listing not found.");

            if (listing.Status == ListingStatus.Open)
                return Result<ListingResponse>.Ok(MapToResponse(listing));

            bool isOwner = owner != null && listing.OwnerId == owner.Id;
            bool isCareGiver = careGiver != null && listing.CareGiverId == careGiver.Id;

            if (!isOwner && !isCareGiver)
                return Result<ListingResponse>.Unauthorized();

            return Result<ListingResponse>.Ok(MapToResponse(listing));
        }

        public Result<IEnumerable<ListingResponse>> GetMyListings()
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (owner == null && careGiver == null)
                return Result<IEnumerable<ListingResponse>>.NotFound("No profile found. Please create an Owner or Caregiver profile first.");

            int? ownerId = owner?.Id;
            int? careGiverId = careGiver?.Id;

            var listings = _db.Listings
                .Include(x => x.Pet)
                .Where(x =>
                    (ownerId != null && x.OwnerId == ownerId) ||
                    (careGiverId != null && x.CareGiverId == careGiverId))
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Result<IEnumerable<ListingResponse>>.Ok(listings.Select(MapToResponse));
        }

        public Result<ListingResponse> UpdateListing(int id, UpdateListingRequest request)
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (owner == null && careGiver == null)
                return Result<ListingResponse>.NotFound("No profile found.");

            var listing = _db.Listings.Include(x => x.Pet).FirstOrDefault(x => x.Id == id);
            if (listing == null)
                return Result<ListingResponse>.NotFound("Listing not found.");

            bool isAuthorized = false;
            if (listing.ListingType == ListingType.OwnerNeedsCareGiver && owner != null)
                isAuthorized = listing.OwnerId == owner.Id;
            else if (listing.ListingType == ListingType.CareGiverOffersService && careGiver != null)
                isAuthorized = listing.CareGiverId == careGiver.Id;

            if (!isAuthorized)
                return Result<ListingResponse>.BadRequest("You do not have permission to edit this listing.");

            if (listing.Status == ListingStatus.Closed)
                return Result<ListingResponse>.BadRequest("Cannot update a closed listing.");

            if (request.Title != null) listing.Title = request.Title;
            if (request.Description != null) listing.Description = request.Description;
            if (request.Status.HasValue) listing.Status = request.Status.Value;
            if (request.StartDate.HasValue) listing.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue) listing.EndDate = request.EndDate.Value;
            if (request.ProposedBudget.HasValue) listing.ProposedBudget = request.ProposedBudget.Value;
            if (request.PetName != null) listing.PetName = request.PetName;

            _db.SaveChanges();

            // ── Invalidate cache — listing updated ────────────────────────
            _cache.Remove(ALL_LISTINGS_KEY);

            return Result<ListingResponse>.Ok(MapToResponse(listing));
        }

        private ListingResponse MapToResponse(Models.Listing listing) => new ListingResponse
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            ListingType = listing.ListingType,
            Status = listing.Status,
            StartDate = listing.StartDate,
            EndDate = listing.EndDate,
            CreatedAt = listing.CreatedAt,
            ProposedBudget = listing.ProposedBudget,
            PetName = listing.Pet?.PetName ?? listing.PetName ?? "No Pet Assigned",
            OwnerId = listing.OwnerId,
            CareGiverId = listing.CareGiverId,
            PetId = listing.PetId,
        };
    }
}
