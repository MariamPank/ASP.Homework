using OfficeSpaceRent.Common.Entity;
using OfficeSpaceRent.Enums;
using OfficeSpaceRent.Models;
using System.ComponentModel.DataAnnotations;

namespace OfficeSpaceRent.Models
{
    public class RentalRequest : Entity
    {
        // Office & User => Many-to-Many
        public int UserId { get; set; }
        public User User { get; set; }

        public int OfficeSpaceId { get; set; }
        public OfficeSpace OfficeSpace { get; set; }


        public DateTime DesiredStartDate { get; set; }
        public int DurationMonths { get; set; }
        public string Message { get; set; } = string.Empty;
        public RentalRequestStatus Status { get; set; } = RentalRequestStatus.Pending;
    }
}