using _4Paws.DTOs.Review.Requests;
using FluentValidation;

namespace _4Paws.Validators.Review
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewRequest>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.AgreementId)
                .GreaterThan(0).WithMessage("AgreementId is required.");

            RuleFor(x => x.Rating)
                .IsInEnum().WithMessage("Rating must be between 1 (VeryBad) and 5 (Excellent).");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.")
                .When(x => x.Comment != null);

            // Exactly one target must be provided
            RuleFor(x => x)
                .Must(x =>
                {
                    int targets = (x.OwnerId.HasValue ? 1 : 0)
                                + (x.CareGiverId.HasValue ? 1 : 0)
                                + (x.PetId.HasValue ? 1 : 0);
                    return targets == 1;
                })
                .WithMessage("Exactly one target must be provided: OwnerId, CareGiverId, or PetId.");
        }
    }
}
