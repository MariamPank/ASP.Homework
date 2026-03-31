using _4Paws.Common.Results;
using _4Paws.Common.Services;
using _4Paws.Data;
using _4Paws.DTOs.Auth.Requests;
using _4Paws.DTOs.Auth.Responses;
using _4Paws.Models;
using _4Paws.Validators.Auth;


namespace _4Paws.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _db;

        private readonly SmtpService _smtp;

        private readonly JwtService _jwt;

        public AuthService (DataContext db, SmtpService smtp, JwtService jwt)
        {
            _db = db;
            _smtp = smtp;
            _jwt = jwt;
        }

        public Result<int> Register(RegisterRequest req)
        {
            RegisterValidator validator = new RegisterValidator();

            var result = validator.Validate(req);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<int>.ValidationError(errors);
            }

            if (_db.Users.Any(u => u.Email == req.Email))
                return Result<int>.BadRequest("user already exists.");

            string hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            UserModel user = new UserModel (req.Username, req.Email, req.Password);

            Random rand = new Random();
            user.VerificationCode = rand.Next(100_000, 999_999).ToString();

            _db.Users.Add(user);
            _db.SaveChanges();

            string body = $"Verification code: {user.VerificationCode}";

            _smtp.SendEmail("Email verification", user.Email, body);


            return Result<int>.Ok(user.Id);
        }

        public Result<TokenResponse> VerifyEmail(VerifyEmailRequest req)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == req.Email);

            if (user == null)
                return Result<TokenResponse>.NotFound("user not found");

            if (user.VerificationCode != req.Code)
                return Result<TokenResponse>.BadRequest("Verification code is not correct");

            user.IsVerified = true;
            user.VerificationCode = null;

            _db.SaveChanges();

            string accessToken = _jwt.GenerateJwtToken(user);

            return Result<TokenResponse>.Ok(new TokenResponse(accessToken));
        }

        public Result<TokenResponse> Login(LoginRequest req)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == req.Email);

            if (user == null)
                return Result<TokenResponse>.BadRequest("email or password is not correct.");
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Result<TokenResponse>.BadRequest("email or password is not correct.");

            if (!user.IsVerified)
            {
                Random rand = new Random();
                user.VerificationCode = rand.Next(100_000, 999_999).ToString();

                _db.SaveChanges();

                string body = $"Verification code: {user.VerificationCode}";

                _smtp.SendEmail("Email verification", user.Email, body);


                return Result<TokenResponse>.Ok(new TokenResponse("Verification"));
            }

            string accessToken = _jwt.GenerateJwtToken(user);

            return Result<TokenResponse>.Ok(new TokenResponse(accessToken));
        }

        public Result<int> ForgotPassword(string email)
        {
            var user = _db.Users
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
                return Result<int>.BadRequest("If email exists you will get verification code");

            if (!user.IsVerified)
                return Result<int>.BadRequest("If email exists you will get verification code");


            Random rand = new Random();
            user.VerificationCode = rand.Next(100_000, 999_999).ToString();

            _db.SaveChanges();

            string body = $"Verification code: {user.VerificationCode}";

            _smtp.SendEmail("Email verification", user.Email, body);

            return Result<int>.Ok(user.Id);
        }

        public Result<int> ResetPassword(ResetPasswordRequest req)
        {
            var user = _db.Users
                .FirstOrDefault(u => u.Email == req.Email);

            if (user == null)
                return Result<int>.NotFound("user not found.");

            if (user.VerificationCode != req.Code)
                return Result<int>.BadRequest("Verification code is not correct.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            user.VerificationCode = null;
            _db.SaveChanges();

            return Result<int>.Ok(user.Id);
        }
    }
}
