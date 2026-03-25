using OfficeSpaceRent.Common.Entity;
using OfficeSpaceRent.Enums;

namespace OfficeSpaceRent.Models
{
    public class Lease : Entity
    {
        //Office & User => Many-to-Many
        public int UserId { get; set; }
        public User User { get; set; }

        public int OfficeSpaceId { get; set; }
        public OfficeSpace OfficeSpace { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }

        public LeaseStatus Status { get; set; } = LeaseStatus.Active;
    }
}
