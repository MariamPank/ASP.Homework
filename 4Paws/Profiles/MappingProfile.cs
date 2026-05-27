using _4Paws.DTOs.Admin.Responses;
using _4Paws.DTOs.Caregiver.Responses;
using _4Paws.DTOs.Owner.Responses;
using _4Paws.DTOs.Pet.Responses;
using _4Paws.Models;
using AutoMapper;

namespace _4Paws.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pet, PetResponse>();
            CreateMap<Pet, CreatePetResponse>();

            CreateMap<Owner, CreateOwnerProfileResponse>();

            CreateMap<CareGiver, CreateCaregiverProfileResponse>();

            CreateMap<UserModel, AdminUserResponse>()
                .ForMember(dest => dest.HasOwnerProfile, opt => opt.MapFrom(src => src.Owner != null))
                .ForMember(dest => dest.HasCareGiverProfile, opt => opt.MapFrom(src => src.CareGiver != null));
        }
    }
}
