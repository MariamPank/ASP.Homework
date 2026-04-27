using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Listing.Requests;
using _4Paws.DTOs.Listing.Responses;
using _4Paws.Enums;
using _4Paws.Helper.CareGiver;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Services.Listing
{
    public class ListingService : IListingService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;
        private readonly ICurrentCareGiver _currentCareGiver;

        public ListingService(DataContext db, ICurrentOwner currentOwner, ICurrentCareGiver currentCaregiver)
        {
            _db = db;
            _currentOwner = currentOwner;
            _currentCareGiver = currentCaregiver;
        }

        public Result<ListingResponse> CreateListing(CreateListingRequest request)
        {
            if (request == null)
                return Result<ListingResponse>.BadRequest("Request is null");

            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (request.ListingType == ListingType.OwnerNeedsCareGiver)
            {
                if (owner == null) return Result<ListingResponse>.NotFound("Owner profile not found. You must create an owner profile first.");

                if (!request.PetId.HasValue)
                    return Result<ListingResponse>.BadRequest("PetId is required for owner listings.");

                // Check for duplicate Open listings for this specific pet
                if (_db.Listings.Any(x => x.OwnerId == owner.Id && x.PetId == request.PetId && x.Status == ListingStatus.Open))
                    return Result<ListingResponse>.BadRequest("An open listing for this pet already exists.");
            }
            else if (request.ListingType == ListingType.CareGiverOffersService)
            {
                if (careGiver == null) return Result<ListingResponse>.NotFound("Caregiver profile not found. You must create a caregiver profile first.");

                // Check for duplicate Open listings for this caregiver
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

                // Logic: Use OwnerId if Owner, CareGiverId if CareGiver
                OwnerId = (request.ListingType == ListingType.OwnerNeedsCareGiver) ? owner.Id : null,
                PetId = (request.ListingType == ListingType.OwnerNeedsCareGiver) ? request.PetId : null,
                CareGiverId = (request.ListingType == ListingType.CareGiverOffersService) ? careGiver.Id : null
            };

            _db.Listings.Add(listing);
            _db.SaveChanges();

            var response = new ListingResponse
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
                PetName = request.PetName
            };

            return Result<ListingResponse>.Ok(response);
        }

        public Result<bool> DeleteListing(int id)
        {
            // 1. Fetch the listing first to see what kind it is
            var listingToDelete = _db.Listings.FirstOrDefault(x => x.Id == id);

            if (listingToDelete == null)
                return Result<bool>.NotFound("Listing not found.");

            // 2. Check ownership based on the ListingType
            if (listingToDelete.ListingType == ListingType.OwnerNeedsCareGiver)
            {
                var owner = _currentOwner.GetCurrentOwner();

                // Ensure the person is an Owner AND they own this specific listing
                if (owner == null || listingToDelete.OwnerId != owner.Id)
                    return Result<bool>.BadRequest("You do not have permission to delete this owner listing.");
            }
            else if (listingToDelete.ListingType == ListingType.CareGiverOffersService)
            {
                var careGiver = _currentCareGiver.GetCurrentCareGiver();

                // Ensure the person is a Caregiver AND they own this specific listing
                if (careGiver == null || listingToDelete.CareGiverId != careGiver.Id)
                    return Result<bool>.BadRequest("You do not have permission to delete this service offer.");
            }

            // 3. Perform the delete
            _db.Listings.Remove(listingToDelete);
            _db.SaveChanges();

            return Result<bool>.Ok(true);
        }

        public Result<IEnumerable<ListingResponse>> GetAllActiveListings()
        {
            var listings = _db.Listings.Where(x=>x.Status == ListingStatus.Open).ToList();

            var responseList = new List<ListingResponse>();

            foreach (var listing in listings)
            {
                var response = new ListingResponse
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
                    PetName = listing.PetName,
                };
                responseList.Add(response);
            }


            return Result<IEnumerable<ListingResponse>>.Ok(responseList);

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

            bool isMyOwnerListing = owner != null && listing.OwnerId == owner.Id;
            bool isMyCareGiverListing = careGiver != null && listing.CareGiverId == careGiver.Id;

            if (!isMyOwnerListing && !isMyCareGiverListing)
            {
                return Result<ListingResponse>.BadRequest("You do not have permission to view this private listing details.");
            }

            var response = new ListingResponse
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
                PetName = listing.Pet?.PetName ?? "No Pet Assigned"
            };

            return Result<ListingResponse>.Ok(response);
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
                    (careGiverId != null && x.CareGiverId == careGiverId)
                )
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var response = listings.Select(listing => new ListingResponse
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
                PetName = listing.Pet?.PetName ?? "N/A"
            });

            return Result<IEnumerable<ListingResponse>>.Ok(response);
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
            {
                isAuthorized = listing.OwnerId == owner.Id;
            }
            else if (listing.ListingType == ListingType.CareGiverOffersService && careGiver != null)
            {
                isAuthorized = listing.CareGiverId == careGiver.Id;
            }

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

            var response = new ListingResponse
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
                PetName = listing.Pet?.PetName ?? listing.PetName
            };

            return Result<ListingResponse>.Ok(response);
        }
    }
}
