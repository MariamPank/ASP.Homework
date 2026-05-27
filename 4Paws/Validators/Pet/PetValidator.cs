using _4Paws.DTOs.Listing.Requests;
using _4Paws.DTOs.Pet.Requests;
using FluentValidation;

namespace _4Paws.Validators.Pet
{
    public class PetValidator : AbstractValidator<CreatePetRequest>
    {
        public PetValidator()
        {
            RuleFor(x => x.PetName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        }
    }
}