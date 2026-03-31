using _4Paws.Enums;

namespace _4Paws.DTOs.Owner.Responses
{
    public class OwnerListingShortResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ListingStatus Status { get; set; }
    }
}
