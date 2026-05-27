using _4Paws.Enums;

namespace _4Paws.DTOs.User.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public string? AvatarUrl { get; set; }  // ← new
    }
}
