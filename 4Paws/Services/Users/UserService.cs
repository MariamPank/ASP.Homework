using _4Paws.Common.Results;
using _4Paws.Common.Services;
using _4Paws.Data;
using _4Paws.DTOs.User.Requests;
using _4Paws.DTOs.User.Responses;
using _4Paws.Services.Users;

namespace _4Paws.Services.User
{
    public class UserService : IUserService
    {
        private readonly DataContext _db;

        public UserService(DataContext db) => _db = db;

        public Result<UserResponse> GetById(int userId)
        {
            var user = _db.Users.Find(userId);

            if (user == null)
                return Result<UserResponse>.NotFound("User not found");

            return Result<UserResponse>.Ok(new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl,
            });
        }

        public Result<int> ChangePassword(int userId, ChangePasswordRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId && u.IsVerified);
            if (user == null) return Result<int>.Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                return Result<int>.BadRequest("Old password is not correct! try again.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _db.SaveChanges();

            return Result<int>.Ok(user.Id);
        }

        public Result<int> DeleteUser(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId && u.IsVerified);
            if (user == null) return Result<int>.Unauthorized();

            _db.Users.Remove(user);
            _db.SaveChanges();

            return Result<int>.Ok(user.Id);
        }

        public Result<int> EditUser(int userId, EditUserRequest request)
        {
            if (request == null)
                return Result<int>.BadRequest("Request is null");

            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Result<int>.NotFound("User not found");
            if (!user.IsVerified) return Result<int>.Unauthorized();

            var emailExists = _db.Users.Any(u => u.Email == request.Email && u.Id != userId);
            if (emailExists) return Result<int>.BadRequest("Email is already in use");

            if (request.UserName != null) user.FullName = request.UserName;
            if (request.Email != null) user.Email = request.Email;
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;

            _db.SaveChanges();
            return Result<int>.Ok(user.Id);
        }

        // ── Avatar ────────────────────────────────────────────────────────

        public Result<string> UpdateAvatar(int userId, string avatarUrl)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return Result<string>.NotFound("User not found");

            user.AvatarUrl = avatarUrl;
            _db.SaveChanges();

            return Result<string>.Ok(avatarUrl);
        }

        public Result<int> DeleteAvatar(int userId, FileUploadService fileUpload)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return Result<int>.NotFound("User not found");

            // Delete physical file from wwwroot
            fileUpload.DeleteImage(user.AvatarUrl);

            user.AvatarUrl = null;
            _db.SaveChanges();

            return Result<int>.Ok(userId);
        }
    }
}
