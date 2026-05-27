using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Application.Requests;
using _4Paws.DTOs.Application.Responses;
using _4Paws.Enums;
using _4Paws.Helper.CareGiver;
using _4Paws.Helper.Owner;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Services.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;
        private readonly ICurrentCareGiver _currentCareGiver;

        public ApplicationService(DataContext db, ICurrentOwner currentOwner, ICurrentCareGiver currentCareGiver)
        {
            _db = db;
            _currentOwner = currentOwner;
            _currentCareGiver = currentCareGiver;
        }

        public Result<ApplicationResponse> ApplyToListing(ApplyRequest request)
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            var listing = _db.Listings.FirstOrDefault(x => x.Id == request.ListingId);
            if (listing == null || listing.Status != ListingStatus.Open)
                return Result<ApplicationResponse>.BadRequest("Listing is no longer available.");

            int? applicantOwnerId = null;
            int? applicantCareGiverId = null;
            string applicantName = "Unknown";

            if (listing.ListingType == ListingType.OwnerNeedsCareGiver)
            {
                if (careGiver == null)
                    return Result<ApplicationResponse>.BadRequest("You need a Caregiver profile to apply for this job.");

                applicantCareGiverId = careGiver.Id;
                applicantName = careGiver.CareGiverName; 
            }
            else
            {
                if (owner == null)
                    return Result<ApplicationResponse>.BadRequest("You need an Owner profile to request this service.");

                applicantOwnerId = owner.Id;
                applicantName = owner.User.FullName;
            }

            bool isMyOwnListing = (listing.OwnerId != null && listing.OwnerId == applicantOwnerId) ||
                                  (listing.CareGiverId != null && listing.CareGiverId == applicantCareGiverId);

            if (isMyOwnListing)
                return Result<ApplicationResponse>.BadRequest("You cannot apply to your own listing.");

            var alreadyApplied = _db.Applications.Any(x => x.ListingId == request.ListingId &&
                ((applicantOwnerId != null && x.OwnerId == applicantOwnerId) ||
                 (applicantCareGiverId != null && x.CareGiverId == applicantCareGiverId)));

            if (alreadyApplied)
                return Result<ApplicationResponse>.BadRequest("You have already applied to this listing.");

            var application = new Models.Application
            {
                ListingId = request.ListingId,
                Message = request.Message,
                Status = ApplicationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ProposedFee = request.ProposedFee,
                OwnerId = applicantOwnerId,
                CareGiverId = applicantCareGiverId
            };

            _db.Applications.Add(application);
            _db.SaveChanges();

            return Result<ApplicationResponse>.Ok(MapToResponse(application));
        }

        public Result<IEnumerable<ApplicationResponse>> GetApplicationsForListing(int listingId)
        {
            // 1. Get both profile contexts
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (owner == null && careGiver == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("No profile found.");

            // 2. Fetch the listing
            var listing = _db.Listings.FirstOrDefault(x => x.Id == listingId);
            if (listing == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("Listing not found.");

            // 3. SECURITY: Verify the requester is the CREATOR of this specific listing
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
                return Result<IEnumerable<ApplicationResponse>>.BadRequest("You do not have permission to view these applications.");

            // 4. Fetch applications with related data
            var applications = _db.Applications
                .Include(x => x.Owner)
                .Include(x => x.CareGiver)
                .Where(x => x.ListingId == listingId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            // 5. Map to response using LINQ Select (cleaner than foreach)
            var response = applications.Select(MapToResponse);

            return Result<IEnumerable<ApplicationResponse>>.Ok(response);
        }

        public Result<IEnumerable<ApplicationResponse>> GetMyApplications()
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            if (owner == null && careGiver == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("No profile found. Please create an Owner or Caregiver profile first.");

            int? ownerId = owner?.Id;
            int? careGiverId = careGiver?.Id;

            var applications = _db.Applications
                .Include(x => x.Owner)
                .Include(x => x.CareGiver)
                .Where(x =>
                    (ownerId != null && x.OwnerId == ownerId) ||
                    (careGiverId != null && x.CareGiverId == careGiverId)
                )
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var response = applications.Select(MapToResponse);

            return Result<IEnumerable<ApplicationResponse>>.Ok(response);
        }

        public Result<ApplicationResponse> UpdateApplicationStatus(int applicationId, ApplicationStatus status)
        {
            var owner = _currentOwner.GetCurrentOwner();
            var careGiver = _currentCareGiver.GetCurrentCareGiver();

            var app = _db.Applications
                .Include(x => x.Owner).ThenInclude(u => u.User)
                .Include(x => x.CareGiver)
                .FirstOrDefault(x => x.Id == applicationId);

            if (app == null) return Result<ApplicationResponse>.NotFound("Application not found.");

            // 3. Fetch the listing
            var listing = _db.Listings.FirstOrDefault(x => x.Id == app.ListingId);
            if (listing == null) return Result<ApplicationResponse>.NotFound("Listing not found.");

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
                return Result<ApplicationResponse>.BadRequest("Only the listing creator can update the application status.");

            app.Status = status;

            if (status == ApplicationStatus.Accepted)
            {
                listing.Status = ListingStatus.Closed;

                var otherApps = _db.Applications
                    .Where(x => x.ListingId == listing.Id && x.Id != applicationId && x.Status == ApplicationStatus.Pending)
                    .ToList();

                foreach (var otherApp in otherApps)
                {
                    otherApp.Status = ApplicationStatus.Rejected;
                }
            }

            _db.SaveChanges();

            //return Result<ApplicationResponse>.Ok(new ApplicationResponse
            //{
            //    Id = app.Id,
            //    ListingId = app.ListingId,
            //    ApplicantName = app.Owner?.User?.FullName ?? app.CareGiver?.CareGiverName ?? "Unknown",
            //    Message = app.Message,
            //    Status = app.Status,
            //    CreatedAt = app.CreatedAt
            //});

            return Result<ApplicationResponse>.Ok(MapToResponse(app));
        }


        private ApplicationResponse MapToResponse(Models.Application application)
        {
            return new ApplicationResponse
            {
                Id = application.Id,
                ListingId = application.ListingId,
                ApplicantName = application.Owner?.User?.FullName ?? application.CareGiver?.CareGiverName ?? "Unknown",
                Message = application.Message,
                Status = application.Status,
                CreatedAt = application.CreatedAt
            };
        }
    }
}
