using _4Paws.Common.Results;
using _4Paws.DTOs.Admin.Responses;
using _4Paws.DTOs.Agreement.Responses;
using _4Paws.DTOs.Application.Responses;
using _4Paws.DTOs.Listing.Responses;

namespace _4Paws.Services.Admin
{
    public interface IAdminService
    {
        // ── Users ─────────────────────────────────────────────────────────
        Result<IEnumerable<AdminUserResponse>> GetAllUsers();
        Result<AdminUserResponse> GetUserById(int userId);
        Result<int> DeleteUser(int userId);
        Result<int> BanUser(int userId);
        Result<int> UnbanUser(int userId);

        // ── Listings ──────────────────────────────────────────────────────
        Result<IEnumerable<ListingResponse>> GetAllListings();
        Result<int> DeleteListing(int listingId);

        // ── Applications ──────────────────────────────────────────────────
        Result<IEnumerable<ApplicationResponse>> GetAllApplications();

        // ── Agreements ────────────────────────────────────────────────────
        Result<IEnumerable<AgreementResponse>> GetAllAgreements();

        // ── Stats ─────────────────────────────────────────────────────────
        Result<AdminStatsResponse> GetStats();
    }
}
