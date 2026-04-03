using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Listing.Requests;
using _4Paws.DTOs.Listing.Responses;
using _4Paws.DTOs.Pet.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using _4Paws.Models;
using Azure.Core;
using System.Reflection;

namespace _4Paws.Services.Listing
{
    public class ListingService : IListingService
    {
        private readonly DataContext _db;
        private readonly ICurrentUserService _currentUser;
        private readonly ICurrentOwner _currentOwner;

        public ListingService(DataContext db, ICurrentUserService currentUser, ICurrentOwner currentOwner)
        {
            _db = db;
            _currentUser = currentUser;
            _currentOwner = currentOwner;
        }

        public Result<ListingResponse> CreateListing(CreateListingRequest request)
        {
            if (request == null)
                return Result<ListingResponse>.BadRequest("Request is null");

            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<ListingResponse>.NotFound("Owner/Caregiver profile not found");

            if (request.ListingType == ListingType.OwnerNeedsCareGiver && !request.PetId.HasValue)
            {
                return Result<ListingResponse>.BadRequest("PetId is required for this listing type.");
            }

            var listingExists = _db.Listings.Any(x =>
                x.OwnerId == owner.Id &&
                x.PetId == request.PetId &&
                x.Status == ListingStatus.Open);

            if (listingExists)
                return Result<ListingResponse>.BadRequest("An open listing for this pet already exists.");

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
                CareGiverId = (request.ListingType == ListingType.CareGiverOffersService) ? owner.Id : null
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
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<bool>.NotFound("Owner profile not found");

            var listingToDelete = _db.Listings.FirstOrDefault(x => x.Id == id && x.OwnerId == owner.Id);
            if (listingToDelete == null)
                return Result<bool>.NotFound("Listing not found");

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
            if (owner == null)
                return Result<ListingResponse>.NotFound("Owner/Caregiver profile not found");

            var listing = _db.Listings.FirstOrDefault(x => x.Id == id &&
                                                     (x.OwnerId == owner.Id || x.CareGiverId == owner.Id));

            if (listing == null)
                return Result<ListingResponse>.NotFound("Listing not found or you don't have permission to view it");

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
                PetName = listing.PetName
            };

            return Result<ListingResponse>.Ok(response);
        }

        public Result<IEnumerable<ListingResponse>> GetMyListings()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<IEnumerable<ListingResponse>>.NotFound("Owner/Caregiver profile not found");

            var listings = _db.Listings.Where (x => x.OwnerId == owner.Id || x.CareGiverId == owner.Id).ToList();

            if (listings == null)
                return Result<IEnumerable<ListingResponse>>.NotFound("Listing not found or you don't have permission to view it");

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

        public Result<ListingResponse> UpdateListing(int id, UpdateListingRequest request)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<ListingResponse>.NotFound("Owner profile not found");

            var listing = _db.Listings.FirstOrDefault(x => x.Id == id && (x.OwnerId == owner.Id || x.CareGiverId == owner.Id));

            if (listing == null)
                return Result<ListingResponse>.NotFound("Listing not found");

            if (listing.Status == ListingStatus.Closed)
                return Result<ListingResponse>.BadRequest("Cannot update a closed listing.");

            if (request.PetName != null)
            {
                listing.PetName = request.PetName;
            }

            if (request.Title != null)
            {
                listing.Title = request.Title;
            }

            if (request.Description != null)
            {
                listing.Description = request.Description;
            }

            if (request.Status.HasValue)
            {
                listing.Status = request.Status.Value;
            }

            if (request.StartDate.HasValue)
            {
                listing.StartDate = request.StartDate.Value;
            }

            if (request.EndDate.HasValue)
            {
                listing.EndDate = request.EndDate.Value;
            }

            if (request.ProposedBudget.HasValue)
            {
                listing.ProposedBudget = request.ProposedBudget.Value;
            }


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
                PetName = listing.PetName,
            };

            return Result<ListingResponse>.Ok(response);
        }
    }
}
