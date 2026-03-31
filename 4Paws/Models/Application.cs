using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class Application : Entity
    {
        public string Message { get; set; }
        public decimal ProposedFee { get; set; }
        public ApplicationStatus Status { get; set; }

        public int ListingId { get; set; }
        public Listing Listing { get; set; }

        public int? OwnerId { get; set; }
        public Owner? Owner { get; set; }

        public int? CareGiverId { get; set; }
        public CareGiver? CareGiver { get; set; }

        public List<Agreement> Agreements { get; set; } = new();
    }
}
