using _4Paws.DTOs.Auth.Requests;
using _4Paws.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _4Paws.Controllers
{
    [Route("api/auth"), ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest req)
        {
            var result = _auth.Register(req);

            return StatusCode(result.Status, result);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest req)
        {
            var result = _auth.Login(req);

            return StatusCode(result.Status, result);
        }

        [HttpPut("verify-email")]
        public IActionResult Verify(VerifyEmailRequest req)
        {
            var result = _auth.VerifyEmail(req);

            return StatusCode(result.Status, result);
        }

        [HttpPost("forgot-password/{email}")]
        public IActionResult ForgotPassword(string email)
        {
            var result = _auth.ForgotPassword(email);

            return StatusCode(result.Status, result);
        }

        [HttpPut("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest req)
        {
            var result = _auth.ResetPassword(req);

            return StatusCode(result.Status, result);
        }
    }
}
