using _4Paws.Common.Results;
using _4Paws.Common.Services;
using _4Paws.Data;
using _4Paws.DTOs.Caregiver.Requests;
using _4Paws.DTOs.Caregiver.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Services;
using Azure;
using Microsoft.EntityFrameworkCore;


namespace _4Paws.Services.CareGiver
{
    public class CaregiverService : ICaregiverService
    {
        private readonly DataContext _db;

        private readonly JwtService _jwt;
        private readonly ICurrentUserService _currentUser;

        public CaregiverService(DataContext db, JwtService jwt, ICurrentUserService currentUser)
        {
            _db = db;
            _jwt = jwt;
            _currentUser = currentUser;
        }
        public Result<CreateCaregiverProfileResponse> CreateCaregiverProfile(CreateCaregiverProfileRequest request)
        {
            if (request == null)
                return Result<CreateCaregiverProfileResponse>.BadRequest("Request is null");

            // 1. Get current userId (JWT-დან)
            var userId = _currentUser.CurrentUserId();

            // 2. Check if user exists
            var userExists = _db.Users.Any(x => x.Id == userId);
            if (!userExists)
                return Result<CreateCaregiverProfileResponse>.NotFound("User not found");

            // 3. Check if Caregiver profile already exists
            var caregiverExists = _db.CareGivers.Any(x => x.UserId == userId);
            if (caregiverExists)
                return Result<CreateCaregiverProfileResponse>.BadRequest("Caregiver profile already exists");

            // 4. Create Caregiver
            var caregiver = new Models.CareGiver
            {
                CareGiverName = request.UserName,
                CareGiverRating = Rating.Average, // default
                UserId = userId,
            };

            _db.CareGivers.Add(caregiver);
            _db.SaveChanges();

            // 5. Map response
            var response = new CreateCaregiverProfileResponse
            {
                Id = caregiver.Id,
                UserName = caregiver.CareGiverName,
                CaregiverRating = caregiver.CareGiverRating,
                UserId = caregiver.UserId
            };

            return Result<CreateCaregiverProfileResponse>.Ok(response);
        }


        public Result<GetCaregiverByIdResponse> GetCaregiverById(int caregiverId)
        {
            var caregiverExists = _db.CareGivers.Any(x => x.Id == caregiverId);

            if (!caregiverExists)
                return Result<GetCaregiverByIdResponse>.NotFound("Caregiver not found");

            var caregiver = _db.CareGivers.Where(x => x.Id == caregiverId)
                .Select(x => new GetCaregiverByIdResponse
                {
                    Id = x.Id,
                    UserName = x.CareGiverName,
                    CaregiverRating = x.CareGiverRating,
                    UserId = x.UserId
                })
                .FirstOrDefault();

            return Result<GetCaregiverByIdResponse>.Ok(caregiver);
        }

        public Result<GetCaregiverDashboardResponse> GetCaregiverDashboard(int caregiverId)
        {
            var caregiver = _db.CareGivers
                .Include(x => x.Listings)
                .Include(x => x.Agreements)
                .FirstOrDefault(x => x.Id == caregiverId);

            if (caregiver == null) return Result<GetCaregiverDashboardResponse>.NotFound("Profile not found");

            var dashboard = _db.CareGivers
                .Where(x => x.Id == caregiverId)
                .Select(x => new GetCaregiverDashboardResponse
                {
                    CaregiverId = x.Id,
                    UserName = x.CareGiverName,
                    CaregiverRating = x.CareGiverRating,
                    TotalListings = x.Listings.Count(),
                    ActiveListings = x.Listings.Count(l => l.Status == ListingStatus.Open),

                    TotalAgreements = x.Agreements.Count(),
                    ActiveAgreements = x.Agreements.Count(a => a.Status == AgreementStatus.Active),
                    CompletedAgreements = x.Agreements.Count(a => a.Status == AgreementStatus.Completed),

                    RecentListings = x.Listings
                        .OrderByDescending(l => l.Id)
                        .Take(5)
                        .Select(l => new CaregiverListingShortResponse
                        {
                            Id = l.Id,
                            Title = l.Title,
                            Status = l.Status
                        })
                        .ToList(),

                    RecentAgreements = x.Agreements
                        .OrderByDescending(a => a.Id)
                        .Take(5)
                        .Select(a => new CaregiverAgreementShortResponse
                        {
                            Id = a.Id,
                            Status = a.Status,
                            PetName = a.Pet.PetName,
                            OwnerName = a.Owner.UserName
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (dashboard == null)
                return Result<GetCaregiverDashboardResponse>.NotFound("Caregiver dashboard not found");

            return Result<GetCaregiverDashboardResponse>.Ok(dashboard);
        }

        public Result<List<GetCaregiverAgreementsResponse>> GetCaregiverAgreements(int caregiverId)
        {
            var caregiverExists = _db.CareGivers.Any(x => x.Id == caregiverId);

            if (!caregiverExists)
                return Result<List<GetCaregiverAgreementsResponse>>.NotFound("Caregiver not found");

            var agreements = _db.Agreements
                .Where(x => x.CareGiverId == caregiverId)
                .Select(x => new GetCaregiverAgreementsResponse
                {
                    Id = x.Id,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    AgreedFee = x.AgreedFee,
                    OwnerName = x.Owner.UserName,
                    PetName = x.Pet.PetName
                })
                .ToList();

            return Result<List<GetCaregiverAgreementsResponse>>.Ok(agreements);
        }

        public Result<List<GetCaregiverListingsResponse>> GetCaregiverListings(int caregiverId)
        {
            var caregiverExists = _db.CareGivers.Any(x => x.Id == caregiverId);

            if (!caregiverExists)
                return Result<List<GetCaregiverListingsResponse>>.NotFound("Caregiver not found");

            var listings = _db.Listings
                .Where(x => x.CareGiverId == caregiverId)
                .Select(x => new GetCaregiverListingsResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    ProposedBudget = x.ProposedBudget,
                })
                .ToList();

            return Result<List<GetCaregiverListingsResponse>>.Ok(listings);
        }
    }
}
