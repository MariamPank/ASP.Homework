using _4Paws.Enums;

namespace _4Paws.DTOs.Listing.Requests
{
    public class UpdateListingRequest
    {
        public string? PetName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ListingStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal? ProposedBudget { get; set; }
    }
}
