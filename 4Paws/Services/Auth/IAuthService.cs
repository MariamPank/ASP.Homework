using _4Paws.Common.Results;
using _4Paws.DTOs.Auth.Requests;
using _4Paws.DTOs.Auth.Responses;


namespace _4Paws.Services.Auth
{
    public interface IAuthService
    {
        Result<int> Register(DTOs.Auth.Requests.RegisterRequest req);
        Result<TokenResponse> Login(DTOs.Auth.Requests.LoginRequest req);
        Result<TokenResponse> VerifyEmail(VerifyEmailRequest req);
        Result<int> ForgotPassword(string email);
        Result<int> ResetPassword(DTOs.Auth.Requests.ResetPasswordRequest req);
        Result<int> ClearUnverifiedUsers();
    }
}
