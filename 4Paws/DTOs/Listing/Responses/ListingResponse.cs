using _4Paws.Enums;

namespace _4Paws.DTOs.Listing.Responses
{
    public class ListingResponse
    {
        public int Id { get; set; }
        public string PetName { get; set; } // Flattened for UI convenience
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public ListingType ListingType { get; set; }
        public ListingStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal ProposedBudget { get; set; }
        public int? OwnerId { get; set; }
        public int? CareGiverId { get; set; }
        public int? PetId { get; set; }
    }
}
