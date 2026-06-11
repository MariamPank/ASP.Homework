using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Agreement.Responses;
using _4Paws.Enums;
using _4Paws.Helper.CareGiver;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Services.Agreement
{
    public class AgreementService : IAgreementService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;
        private readonly ICurrentCareGiver _currentCareGiver;
        private readonly ICurrentUserService _currentUser;

        public AgreementService(DataContext db, ICurrentOwner currentOwner, ICurrentCareGiver currentCareGiver, ICurrentUserService currentUser)
        {
            _db = db;
            _currentOwner = currentOwner;
            _currentCareGiver = currentCareGiver;
            _currentUser = currentUser;
        }

        public Result<AgreementResponse> CreateAgreement(int applicationId)
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (owner == null && careGiver == null)
                return Result<AgreementResponse>.NotFound("Profile not found.");

            var app = _db.Applications
                .Include(x => x.Listing)
                .FirstOrDefault(x => x.Id == applicationId);

            if (app == null) return Result<AgreementResponse>.NotFound("Application not found.");

            if (app.Status != ApplicationStatus.Accepted)
                return Result<AgreementResponse>.BadRequest("Agreement can only be created for accepted applications.");

            var listing = app.Listing;

            bool isAuthorized = false;
            if (listing.ListingType == ListingType.OwnerNeedsCareGiver && owner != null)
                isAuthorized = listing.OwnerId == owner.Id;
            else if (listing.ListingType == ListingType.CareGiverOffersService && careGiver != null)
                isAuthorized = listing.CareGiverId == careGiver.Id;

            if (!isAuthorized)
                return Result<AgreementResponse>.BadRequest("You do not have permission to create this agreement.");

            var agreement = new Models.Agreement
            {
                ListingId = listing.Id,
                ApplicationId = app.Id,

                PetId = (listing.ListingType == ListingType.OwnerNeedsCareGiver ? listing.PetId : app.PetId) ?? 0,
                OwnerId = (listing.ListingType == ListingType.OwnerNeedsCareGiver ? listing.OwnerId : app.OwnerId) ?? 0,
                CareGiverId = (listing.ListingType == ListingType.CareGiverOffersService ? listing.CareGiverId : app.CareGiverId) ?? 0,

                AgreedFee = app.ProposedFee ?? listing.ProposedBudget,
                StartDate = listing.StartDate,
                EndDate = listing.EndDate,
                Status = AgreementStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _db.Agreements.Add(agreement);
            _db.SaveChanges();

            return Result<AgreementResponse>.Ok(MapToResponse(agreement));
        }

        public Result<AgreementResponse> GetAgreementById(int id)
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (owner == null && careGiver == null)
                return Result<AgreementResponse>.NotFound("Profile not found.");

            var agreement = _db.Agreements
                .Include(x => x.Listing)
                .FirstOrDefault(a => a.Id == id);

            if (agreement == null)
                return Result<AgreementResponse>.NotFound("Agreement not found.");


            bool isAuthorized = (owner != null && agreement.OwnerId == owner.Id) ||
                                (careGiver != null && agreement.CareGiverId == careGiver.Id);

            if (!isAuthorized)
                return Result<AgreementResponse>.BadRequest("You do not have permission to view this agreement.");

            return Result<AgreementResponse>.Ok(MapToResponse(agreement));
        }

        public Result<IEnumerable<AgreementResponse>> GetMyAgreements()
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (owner == null && careGiver == null)
                return Result<IEnumerable<AgreementResponse>>.NotFound("Profile not found. Please create an Owner or Caregiver profile first.");

            int? ownerId = owner?.Id;
            int? careGiverId = careGiver?.Id;

            var userId = _currentUser.CurrentUserId(); // inject ICurrentUserService if not already

            var agreements = _db.Agreements
                .Include(x => x.Owner)
                .Include(x => x.CareGiver)
                .Where(x => (ownerId != null && x.OwnerId == ownerId) ||
                            (careGiverId != null && x.CareGiverId == careGiverId))
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var response = agreements.Select(ag => MapToResponse(ag, userId));

            return Result<IEnumerable<AgreementResponse>>.Ok(response);
        }

        public Result<AgreementResponse> CompleteAgreement(int id)
        {
            var owner = _currentOwner.GetCurrentOwner();

            if (owner == null)
                return Result<AgreementResponse>.NotFound("Profile not found");

            var agreement = _db.Agreements.FirstOrDefault(x => x.Id == id && x.OwnerId == owner.Id);
            if (agreement == null) return Result<AgreementResponse>.NotFound("Agreement not found or access denied.");

            if (agreement.Status != AgreementStatus.Active)
                return Result<AgreementResponse>.BadRequest("Only active agreements can be completed.");
            agreement.Status = AgreementStatus.Completed;
            agreement.CompleteAt = DateTime.UtcNow;

            _db.SaveChanges();

            return Result<AgreementResponse>.Ok(MapToResponse(agreement));
        }

        private AgreementResponse MapToResponse(Models.Agreement agreement, int userId = 0)
        {
            bool hasReviewed = userId > 0 && _db.Reviews
                .Any(r => r.AgreementId == agreement.Id && r.ReviewerId == userId);

            return new AgreementResponse
            {
                Id = agreement.Id,
                Status = agreement.Status,
                StartDate = agreement.StartDate,
                EndDate = agreement.EndDate,
                AgreedFee = agreement.AgreedFee,
                OwnerId = agreement.OwnerId,
                CareGiverId = agreement.CareGiverId,
                PetId = agreement.PetId,
                CompleteAt = agreement.CompleteAt,
                OwnerUserId = agreement.Owner?.UserId ?? 0,
                CareGiverUserId = agreement.CareGiver?.UserId ?? 0,
                HasReviewed = hasReviewed,
            };
        }
    }
}
