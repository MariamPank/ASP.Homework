using _4Paws.DTOs.Application.Requests;
using _4Paws.Enums;
using FluentValidation;

namespace _4Paws.Validators.Application
{
    public class ApplyValidator : AbstractValidator<ApplyRequest>
    {
        public ApplyValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required.")
                .MaximumLength(1500).WithMessage("Message cannot exceed 1500 characters.");

            RuleFor(x => x.ProposedFee)
                .GreaterThan(0).WithMessage("Proposed fee must be greater than zero.")
                .When(x => x.ProposedFee.HasValue);
        }
    }
}



