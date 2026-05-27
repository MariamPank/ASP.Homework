using _4Paws.Common.Results;
using _4Paws.Common.Services;
using _4Paws.Data;
using _4Paws.DTOs.Pet.Requests;
using _4Paws.DTOs.Pet.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Owner;
using AutoMapper;

namespace _4Paws.Services.Pet
{
    public class PetService : IPetService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;
        private readonly IMapper _mapper;

        public PetService(DataContext db, ICurrentOwner currentOwner, IMapper mapper)
        {
            _db = db;
            _currentOwner = currentOwner;
            _mapper = mapper;
        }

        public Result<CreatePetResponse> CreatePet(CreatePetRequest request)
        {
            if (request == null)
                return Result<CreatePetResponse>.BadRequest("Request is null");

            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<CreatePetResponse>.NotFound("Owner profile not found");

            var petExists = _db.Pets.Any(x => x.OwnerId == owner.Id && x.PetName == request.PetName);
            if (petExists)
                return Result<CreatePetResponse>.BadRequest("Pet with this name already exists");

            var pet = new Models.Pet
            {
                PetName = request.PetName.Trim(),
                PetRating = Rating.Average,
                Description = request.Description?.Trim(),
                OwnerId = owner.Id
            };

            _db.Pets.Add(pet);
            _db.SaveChanges();

            return Result<CreatePetResponse>.Ok(_mapper.Map<CreatePetResponse>(pet));
        }

        public Result<PetResponse> GetById(int petId)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<PetResponse>.NotFound("Owner profile not found");

            var pet = _db.Pets.FirstOrDefault(x => x.Id == petId && x.OwnerId == owner.Id);
            if (pet == null)
                return Result<PetResponse>.NotFound("Pet not found");

            return Result<PetResponse>.Ok(_mapper.Map<PetResponse>(pet));
        }

        public Result<List<PetResponse>> GetMyPets()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<List<PetResponse>>.NotFound("Owner profile not found");

            var pets = _db.Pets
                .Where(x => x.OwnerId == owner.Id)
                .ToList();

            return Result<List<PetResponse>>.Ok(_mapper.Map<List<PetResponse>>(pets));
        }

        public Result<PetResponse> UpdatePet(int petId, UpdatePetRequest request)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<PetResponse>.NotFound("Owner profile not found");

            var pet = _db.Pets.FirstOrDefault(x => x.Id == petId && x.OwnerId == owner.Id);
            if (pet == null)
                return Result<PetResponse>.NotFound("Pet not found");

            if (request.PetName != null) pet.PetName = request.PetName;
            if (request.Description != null) pet.Description = request.Description;

            _db.SaveChanges();
            return Result<PetResponse>.Ok(_mapper.Map<PetResponse>(pet));
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

        // ── Image ─────────────────────────────────────────────────────────

        public Result<string> UpdatePetImage(int petId, string imageUrl, FileUploadService fileUpload)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<string>.NotFound("Owner profile not found");

            var pet = _db.Pets.FirstOrDefault(x => x.Id == petId && x.OwnerId == owner.Id);
            if (pet == null)
                return Result<string>.NotFound("Pet not found");

            // Delete old image if one exists
            fileUpload.DeleteImage(pet.ImageUrl);

            pet.ImageUrl = imageUrl;
            _db.SaveChanges();

            return Result<string>.Ok(imageUrl);
        }

        public Result<int> DeletePetImage(int petId, FileUploadService fileUpload)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<int>.NotFound("Owner profile not found");

            var pet = _db.Pets.FirstOrDefault(x => x.Id == petId && x.OwnerId == owner.Id);
            if (pet == null)
                return Result<int>.NotFound("Pet not found");

            fileUpload.DeleteImage(pet.ImageUrl);
            pet.ImageUrl = null;
            _db.SaveChanges();

            return Result<int>.Ok(petId);
        }
    }
}
