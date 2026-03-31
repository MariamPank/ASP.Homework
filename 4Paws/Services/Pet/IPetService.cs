using _4Paws.Common.Results;
using _4Paws.DTOs.Pet.Requests;
using _4Paws.DTOs.Pet.Responses;
using System.Net;

namespace _4Paws.Services.Pet
{
    public interface IPetService
    {
        Result<CreatePetResponse> CreatePet(CreatePetRequest request);
        Result<PetResponse> GetById(int petId);
        Result<List<PetResponse>> GetMyPets();
        Result<PetResponse> UpdatePet(int petId, UpdatePetRequest request);
        Result<int> DeletePet(int petId);
    }
}
