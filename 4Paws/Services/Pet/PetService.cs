using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Pet.Requests;
using _4Paws.DTOs.Pet.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;


namespace _4Paws.Services.Pet
{
    public class PetService : IPetService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;

        public PetService(DataContext db, ICurrentUserService currentUser, ICurrentOwner currentOwner)
        {
            _db = db;
            _currentOwner = currentOwner;
        }

        public Result<CreateListingResponse> CreatePet(CreatePetRequest request)
        {
            if (request == null)
                return Result<CreateListingResponse>.BadRequest("Request is null");

            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<CreateListingResponse>.NotFound("Owner profile not found");

            var petExists = _db.Pets.Any(x => x.OwnerId == owner.Id && x.PetName == request.PetName);
            if (petExists)
                return Result<CreateListingResponse>.BadRequest("Pet with this name already exists");

            var pet = new Models.Pet
            {
                PetName = request.PetName.Trim(),
                PetRating = Rating.Average,
                Description = request.Description?.Trim(),
                OwnerId = owner.Id
            };

            _db.Pets.Add(pet);
            _db.SaveChanges();

            var response = new CreateListingResponse
            {
                Id = pet.Id,
                PetName = pet.PetName,
                PetRating = pet.PetRating,
                Description = pet.Description,
                OwnerId = pet.OwnerId
            };

            return Result<CreateListingResponse>.Ok(response);
        }

        public Result<PetResponse> GetById(int petId)
        {
            var owner = _currentOwner.GetCurrentOwner();

            if (owner == null)
                return Result<PetResponse>.NotFound("Owner profile not found");

            var pet = _db.Pets.FirstOrDefault(x => x.Id == petId && x.OwnerId == owner.Id);
            if (pet == null)
                return Result<PetResponse>.NotFound("Pet not found");

            return Result<PetResponse>.Ok(new PetResponse
            {
                Id = pet.Id,
                PetName = pet.PetName,
                PetRating = pet.PetRating,
                Description = pet.Description,
                OwnerId = pet.OwnerId
            });
        }

        public Result<List<PetResponse>> GetMyPets()
        {
            var owner = _currentOwner.GetCurrentOwner();

            if (owner == null)
                return Result<List<PetResponse>>.NotFound("Owner profile not found");

            var pets = _db.Pets
                .Where(x => x.OwnerId == owner.Id)
                .Select(x => new PetResponse
                {
                    Id = x.Id,
                    PetName = x.PetName,
                    PetRating = x.PetRating,
                    Description = x.Description,
                    OwnerId = x.OwnerId
                })
                .ToList();

            return Result<List<PetResponse>>.Ok(pets);
        }

        public Result<PetResponse> UpdatePet(int petId, UpdatePetRequest request)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<PetResponse>.NotFound("Owner profile not found");

            var pet = _db.Pets.FirstOrDefault(x => x.Id == petId && x.OwnerId == owner.Id);
            if (pet == null)
                return Result<PetResponse>.NotFound("Pet not found");

            if (request.PetName != null)
            {
                pet.PetName = request.PetName;
            }

            if (request.Description != null)
            {
                pet.Description = request.Description;
            }

            _db.SaveChanges();

            var response = new PetResponse
            {
                Id = pet.Id,
                PetName = pet.PetName,
                PetRating = pet.PetRating,
                Description = pet.Description,
                OwnerId = pet.OwnerId
            };

            return Result<PetResponse>.Ok(response);
        }

        public Result<int> DeletePet(int petId)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<int>.NotFound("Owner profile not found");

            var petToDelete = _db.Pets.FirstOrDefault(x => x.Id == petId && x.OwnerId == owner.Id);
            if (petToDelete == null)
                return Result<int>.NotFound("Pet not found");

            _db.Pets.Remove(petToDelete);
            _db.SaveChanges();

            return Result<int>.Ok(petId);
        }

    }
}
