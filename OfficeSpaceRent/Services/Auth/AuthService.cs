using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using OfficeSpaceRent.Data;
using OfficeSpaceRent.DTOs.Requests;
using OfficeSpaceRent.DTOs.Responses;
using OfficeSpaceRent.Enums;
using OfficeSpaceRent.Models;
using OfficeSpaceRent.Services.Auth;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OfficeSpaceRent.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _db;
        private readonly IConfiguration _configuration;

        public AuthService(DataContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public AuthResponse Register(DTOs.Requests.RegisterRequest request)
        {
            var existingUser = _db.Users.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
                throw new Exception("User with this email already exists.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.User
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public AuthResponse Login(DTOs.Requests.LoginRequest request)
        {
            var user = _db.Users.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());

            if (user == null)
                throw new Exception("Invalid email or password.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new Exception("Invalid email or password.");

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}