using Microsoft.AspNetCore.Identity.Data;
using OfficeSpaceRent.DTOs.Responses;

namespace OfficeSpaceRent.Services.Auth
{
    public interface IAuthService
    {
        AuthResponse Register(DTOs.Requests.RegisterRequest request);
        AuthResponse Login(DTOs.Requests.LoginRequest request);
    }
}
