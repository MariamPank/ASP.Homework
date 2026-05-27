using FluentValidation;
using LoginRequest = _4Paws.DTOs.Auth.Requests.LoginRequest;

namespace _4Paws.Validators.Auth
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).MinimumLength(6).MaximumLength(50);
        }
    }
}