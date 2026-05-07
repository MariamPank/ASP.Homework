using _4Paws.Enums;

namespace _4Paws.DTOs.Admin.Responses
{
    public class AdminUserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public bool IsVerified { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasOwnerProfile { get; set; }
        public bool HasCareGiverProfile { get; set; }
    }
}
