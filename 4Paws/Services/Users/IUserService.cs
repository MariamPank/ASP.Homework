using _4Paws.Common.Results;
using _4Paws.DTOs.User.Requests;
using _4Paws.DTOs.User.Responses;


namespace _4Paws.Services.Users
{
    public interface IUserService
    {
        Result<UserResponse> GetById(int userId);
        Result<int> ChangePassword(int userId, ChangePasswordRequest request);
        Result<int> EditUser(int userId, EditUserRequest request);
        Result<int> DeleteUser(int userId);
    }
}
