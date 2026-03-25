using _4Paws.Enums;

namespace _4Paws.Models
{
    public class User
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;

        public Owner Owner { get; set; }
        public CareGiver CareGiver { get; set; }
    }
}
