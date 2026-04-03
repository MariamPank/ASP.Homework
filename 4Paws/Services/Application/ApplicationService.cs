using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Application.Requests;
using _4Paws.DTOs.Application.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using Microsoft.EntityFrameworkCore;

namespace _4Paws.Services.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;

        public ApplicationService(DataContext db, ICurrentOwner currentOwner)
        {
            _db = db;
            _currentOwner = currentOwner;
        }
        public Result<ApplicationResponse> ApplyToListing(ApplyRequest request)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<ApplicationResponse>.NotFound("Owner/Caregiver profile not found");

            // 1. Validate the Listing exists and is still open
            var listing = _db.Listings.FirstOrDefault(x => x.Id == request.ListingId);
            if (listing == null || listing.Status != ListingStatus.Open)
                return Result<ApplicationResponse>.BadRequest("Listing is not available for applications.");

            // 2. SAFETY CHECK: Prevents the owner from applying to their own listing
            if (listing.OwnerId == owner.Id || listing.CareGiverId == owner.Id)
                return Result<ApplicationResponse>.BadRequest("You cannot apply to your own listing.");

            // 3. DUPLICATE CHECK: Prevent spamming multiple applications for the same thing
            var alreadyApplied = _db.Applications.Any(x => x.ListingId == request.ListingId && (x.OwnerId == owner.Id || x.CareGiverId == owner.Id));
            if (alreadyApplied)
                return Result<ApplicationResponse>.BadRequest("You have already applied to this listing.");

            // 4. Create the Application
            var application = new Models.Application
            {
                ListingId = request.ListingId,
                Message = request.Message,
                Status = ApplicationStatus.Pending, // Always starts as Pending
                CreatedAt = DateTime.Now,
                ProposedFee = request.PropossedFee,
                OwnerId = (request.AppliedBy == AppliedBy.Owner) ? owner.Id : null,
                CareGiverId = (request.AppliedBy == AppliedBy.Caregiver) ? owner.Id : null
            };

            _db.Applications.Add(application);
            _db.SaveChanges();

            // 5. Map to Response
            var response = new ApplicationResponse
            {
                Id = application.Id,
                ListingId = application.ListingId,
                ApplicantName = owner.UserName,
                Message = application.Message,
                Status = application.Status,
                CreatedAt = application.CreatedAt
            };

            return Result<ApplicationResponse>.Ok(response);
        }

        public Result<IEnumerable<ApplicationResponse>> GetApplicationsForListing(int listingId)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("Owner/Caregiver profile not found");

            var listing = _db.Listings.FirstOrDefault(x => x.Id == listingId);
            if (listing == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("Listing not found");

            // SECURITY: Ensure the person asking is the one who created the listing
            if (listing.OwnerId != owner.Id && listing.CareGiverId != owner.Id)
                return Result<IEnumerable<ApplicationResponse>>.BadRequest("You do not have permission to view these applications.");

            var applications = _db.Applications
                .Include(x => x.Owner)
                .Include(x => x.CareGiver)
                .Where(x => x.ListingId == listingId).ToList();
            if (applications == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("No application found for this listing Id");

            var appsList = new List<ApplicationResponse>();

            foreach (var app in applications)
            {
                var response = new ApplicationResponse
                {
                    Id = app.Id,
                    ListingId = app.ListingId,
                    ApplicantName = app.Owner?.UserName ?? app.CareGiver?.CareGiverName ?? "Unknown",
                    Message = app.Message,
                    Status = app.Status,
                    CreatedAt = app.CreatedAt
                };
                appsList.Add(response);
            }
            return Result<IEnumerable<ApplicationResponse>>.Ok(appsList);
        }

        public Result<IEnumerable<ApplicationResponse>> GetMyApplications()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("Owner/Caregiver profile not found");

            var applications = _db.Applications
                .Include(x => x.Owner)
                .Include(x => x.CareGiver)
                .Where(x => x.OwnerId == owner.Id || x.CareGiverId == owner.Id).ToList();
            if (applications == null)
                return Result<IEnumerable<ApplicationResponse>>.NotFound("No application found");
            var appsList = new List<ApplicationResponse>();

            foreach (var app in applications)
            {
                var response = new ApplicationResponse
                {
                    Id = app.Id,
                    ListingId = app.ListingId,
                    ApplicantName = app.Owner?.UserName ?? app.CareGiver?.CareGiverName ?? "Unknown",
                    Message = app.Message,
                    Status = app.Status,
                    CreatedAt = app.CreatedAt
                };
                appsList.Add(response);
            }

            return Result<IEnumerable<ApplicationResponse>>.Ok(appsList);

        }

        public Result<ApplicationResponse> UpdateApplicationStatus(int applicationId, ApplicationStatus status)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<ApplicationResponse>.NotFound("Owner profile not found");

            var app = _db.Applications.FirstOrDefault(x => x.Id == applicationId);
            if (app == null) return Result<ApplicationResponse>.NotFound("Application not found");

            var listing = _db.Listings.Find(app.ListingId);

            bool isListingOwner = (listing.OwnerId == owner.Id || listing.CareGiverId == owner.Id);

            if (isListingOwner)
            {
                app.Status = status;
            }
            else
            {
                return Result<ApplicationResponse>.BadRequest("Only the listing owner can update the status.");
            }

            // 1. If the Owner ACCEPTS, we close the deal
            if (status == ApplicationStatus.Accepted)
            {
                // Close the listing so no one else can apply
                listing.Status = ListingStatus.Closed;

                // Reject all other pending applications for this listing
                var otherApps = _db.Applications
                    .Include(x => x.Owner)
                    .Include(x => x.CareGiver)
                    .Where(x => x.ListingId == listing.Id && x.Id != applicationId && x.Status == ApplicationStatus.Pending);
                foreach (var otherApp in otherApps)
                {
                    otherApp.Status = ApplicationStatus.Rejected;
                }
            }
            _db.SaveChanges();

            var response = new ApplicationResponse
            {
                Id = app.Id,
                ListingId = app.ListingId,
                ApplicantName = app.Owner?.UserName ?? app.CareGiver?.CareGiverName ?? "Unknown",
                Message = app.Message,
                Status = app.Status,
                CreatedAt = app.CreatedAt
            };

            return Result<ApplicationResponse>.Ok(response);
        }
    }
}
