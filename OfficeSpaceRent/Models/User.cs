using OfficeSpaceRent.Common.Entity;
using OfficeSpaceRent.Enums;
using System.ComponentModel.DataAnnotations;

namespace OfficeSpaceRent.Models
{
    public class User : Entity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public List<RentalRequest> RentalRequests { get; set; } = new();
        public List<Lease> Leases { get; set; } = new();
    }
}
