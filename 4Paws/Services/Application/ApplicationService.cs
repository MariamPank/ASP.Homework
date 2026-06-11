using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Application.Requests;
using _4Paws.DTOs.Application.Responses;
using _4Paws.Enums;
using _4Paws.Helper.CareGiver;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Services.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;
        private readonly ICurrentCareGiver _currentCareGiver;
        private readonly ICurrentUserService _currentUser;

        public ApplicationService(DataContext db, ICurrentOwner currentOwner, ICurrentCareGiver currentCareGiver, ICurrentUserService currentUser)
        {
            _db = db;
            _currentOwner = currentOwner;
            _currentCareGiver = currentCareGiver;
            _currentUser = currentUser;
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
                applicantName = careGiver.User.FullName;
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
            var userId = _currentUser.CurrentUserId();
            if (userId == 0)
                return Result<IEnumerable<ApplicationResponse>>.Unauthorized();

            var listing = _db.Listings
                .Include(x => x.Owner)
                .Include(x => x.CareGiver)
                .FirstOrDefault(x => x.Id == listingId);

            if (listing == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("Listing not found.");

            // Check if the current user created this listing (via either profile)
            bool isAuthorized = (listing.Owner != null && listing.Owner.UserId == userId) ||
                                (listing.CareGiver != null && listing.CareGiver.UserId == userId);

            if (!isAuthorized)
                return Result<IEnumerable<ApplicationResponse>>.BadRequest("You do not have permission to view these applications.");

            var applications = _db.Applications
                .Include(x => x.Owner).ThenInclude(o => o.User)
                .Include(x => x.CareGiver).ThenInclude(c => c.User)
                .Where(x => x.ListingId == listingId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Result<IEnumerable<ApplicationResponse>>.Ok(applications.Select(MapToResponse));
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
                .Include(x => x.Owner).ThenInclude(o => o.User)
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
                .Include(x => x.CareGiver).ThenInclude(c => c.User)
                .FirstOrDefault(x => x.Id == applicationId);

            if (app == null) return Result<ApplicationResponse>.NotFound("Application not found.");

            // 3. Fetch the listing
            var listing = _db.Listings.FirstOrDefault(x => x.Id == app.ListingId);
            if (listing == null) return Result<ApplicationResponse>.NotFound("Listing not found.");

            var userId = _currentUser.CurrentUserId();
            var listingWithProfiles = _db.Listings
                .Include(x => x.Owner)
                .Include(x => x.CareGiver)
                .FirstOrDefault(x => x.Id == app.ListingId);

            bool isAuthorized = listingWithProfiles != null &&
                ((listingWithProfiles.Owner != null && listingWithProfiles.Owner.UserId == userId) ||
                 (listingWithProfiles.CareGiver != null && listingWithProfiles.CareGiver.UserId == userId));

            if (!isAuthorized)
                return Result<ApplicationResponse>.BadRequest("Only the listing creator can update the application status.");

            app.Status = status;

            if (status == ApplicationStatus.Accepted)
            {
                listingWithProfiles.Status = ListingStatus.Closed;

                var otherApps = _db.Applications
                    .Where(x => x.ListingId == listing.Id && x.Id != applicationId && x.Status == ApplicationStatus.Pending)
                    .ToList();

                foreach (var otherApp in otherApps)
                {
                    otherApp.Status = ApplicationStatus.Rejected;
                }
            }

            _db.SaveChanges();

            return Result<ApplicationResponse>.Ok(MapToResponse(app));
        }

        private ApplicationResponse MapToResponse(Models.Application application)
        {
            return new ApplicationResponse
            {
                Id = application.Id,
                ListingId = application.ListingId,
                ApplicantId = application.Owner?.User?.Id ?? application.CareGiver?.User.Id ?? 0,
                ApplicantName = application.Owner?.User?.FullName ?? application.CareGiver?.User.FullName ?? "Unknown",
                Message = application.Message,
                ProposedFee = application.ProposedFee,
                Status = application.Status,
                CreatedAt = application.CreatedAt
            };
        }
    }
}
