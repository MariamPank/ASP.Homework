using _4Paws.Enums;

namespace _4Paws.DTOs.Caregiver.Responses
{
    public class GetCaregiverListingsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ListingStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ProposedBudget { get; set; }
    }
}
