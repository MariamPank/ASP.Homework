using _4Paws.Common.Results;
using _4Paws.DTOs.Owner.Requests;
using _4Paws.DTOs.Owner.Responses;
using _4Paws.DTOs.Pet.Responses;

namespace _4Paws.Services.Owner
{
    public interface IOwnerService
    {
        Result<CreateOwnerProfileResponse> CreateOwnerProfile(CreateOwnerProfileRequest request);
        Result<GetOwnerByIdResponse> GetOwnerById(int ownerId);
        Result<GetOwnerDashboardResponse> GetOwnerDashboard(int ownerId);
        Result<List<GetOwnerListingsResponse>> GetOwnerListings(int ownerId);
        Result<List<GetOwnerAgreementsResponse>> GetOwnerAgreements(int ownerId);
        Result<List<PetResponse>> GetMyPets();

    }
}