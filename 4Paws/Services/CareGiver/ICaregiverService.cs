using _4Paws.Common.Results;
using _4Paws.DTOs.Caregiver.Requests;
using _4Paws.DTOs.Caregiver.Responses;

namespace _4Paws.Services.CareGiver
{
    public interface ICaregiverService
    {
        Result<CreateCaregiverProfileResponse> CreateCaregiverProfile(CreateCaregiverProfileRequest request);
        Result<GetCaregiverByIdResponse> GetCaregiverById(int caregiverId);
        Result<GetCaregiverDashboardResponse> GetCaregiverDashboard(int caregiverId);
        Result<List<GetCaregiverListingsResponse>> GetCaregiverListings(int caregiverId);
        Result<List<GetCaregiverAgreementsResponse>> GetCaregiverAgreements(int caregiverId);
    }
}
