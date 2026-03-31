using _4Paws.Enums;

namespace _4Paws.DTOs.Caregiver.Responses
{
    public class CaregiverListingShortResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ListingStatus Status { get; set; }
    }
}
