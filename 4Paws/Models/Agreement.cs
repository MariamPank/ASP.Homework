using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class Agreement : Entity
    {
        public AgreementStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
        public decimal AgreedFee { get; set; }

        public int OwnerId { get; set; }
        public Owner Owner { get; set; }

        public int CareGiverId { get; set; }
        public CareGiver CareGiver { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }

        public int ListingId { get; set; }
        public Listing Listing { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }
    }
}
