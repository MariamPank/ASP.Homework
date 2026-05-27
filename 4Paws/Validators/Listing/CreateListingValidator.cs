using _4Paws.DTOs.Listing.Requests;
using _4Paws.Enums;
using FluentValidation;

namespace _4Paws.Validators.Listing
{
    public class CreateListingValidator : AbstractValidator<CreateListingRequest>
    {
        public CreateListingValidator() 
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.ProposedBudget).GreaterThan(0);
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        }
    }
}