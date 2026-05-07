using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class UserModel : Entity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public bool IsBanned { get; set; } = false;
        public string? VerificationCode { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        public Owner? Owner { get; set; }
        public CareGiver? CareGiver { get; set; }


        public UserModel() { }

        public UserModel(string username, string email, string pass)
        {
            FullName = username;
            Email = email;
            PasswordHash = pass;
        }
    }
}
