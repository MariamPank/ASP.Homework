using _4Paws.Enums;
using _4Paws.Models;

namespace _4Paws.DTOs.Listing.Requests
{
    public class CreateListingRequest
    {
        public string? PetName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

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
