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
        public Owner Owner { get; set; } = null!;

        public int PetId { get; set; }
        public Pet Pet { get; set; } = null!;

        public int CareGiverId { get; set; }
        public CareGiver CareGiver { get; set; } = null!;
    }
}
