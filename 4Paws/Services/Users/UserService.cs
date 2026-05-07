using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Pet.Responses;
using _4Paws.DTOs.User.Requests;
using _4Paws.DTOs.User.Responses;
using _4Paws.Helper.Owner;
using _4Paws.Helper.Services;
using _4Paws.Services.Users;

namespace _4Paws.Services.User
{
    public class UserService : IUserService
    {
        private readonly DataContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentOwner _currentOwner;

        public UserService (DataContext db, ICurrentUserService currentUserService, ICurrentOwner currentOwner)
        {
            _db = db;
            _currentUserService = currentUserService;
            _currentOwner = currentOwner;
        }
        public Result<UserResponse> GetById(int userId)
        {
            var user = _db.Users.Find(userId);

            if (user == null)
                return Result<UserResponse>.NotFound("User not found");

            var response = new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return Result<UserResponse>.Success(userId, response);
        }
        public Result<int> ChangePassword(int userId, ChangePasswordRequest request)
        {
            var user = _db.Users
                .FirstOrDefault(u => u.Id == userId && u.IsVerified);
            if (user == null) return Result<int>.Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                return Result<int>.BadRequest("Old password is not correct! try again.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _db.SaveChanges();

            return Result<int>.Ok(user.Id);
        }
        public Result<int> DeleteUser(int userId)
        {
            var user = _db.Users
                .FirstOrDefault(u => u.Id == userId && u.IsVerified);

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

            if (user == null)
                return Result<int>.NotFound("User not found");

            if (!user.IsVerified)
                return Result<int>.Unauthorized();

            var emailExists = _db.Users.Any(u => u.Email == request.Email && u.Id != userId);
            if (emailExists)
                return Result<int>.BadRequest("Email is already in use");

            user.FullName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            _db.SaveChanges();

            return Result<int>.Ok(user.Id);
        }
    }
}
