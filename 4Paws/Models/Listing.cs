using _4Paws.Common.Entity;
using _4Paws.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace _4Paws.Models
{
    public class Listing : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public ListingType ListingType { get; set; }
        public ListingStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal ProposedBudget { get; set; }

        public int? OwnerId { get; set; }
        public Owner? Owner { get; set; }

        public int? CareGiverId { get; set; }
        public CareGiver? CareGiver { get; set; }

        public int? PetId { get; set; }
        public Pet? Pet { get; set; }

        public List<Application> Applications { get; set; } = new();
        public List<Agreement> Agreements { get; set; } = new();
    }
}
