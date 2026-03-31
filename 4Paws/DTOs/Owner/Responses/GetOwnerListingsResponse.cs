using _4Paws.Enums;

namespace _4Paws.DTOs.Owner.Responses
{
    public class GetOwnerListingsResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ListingStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ProposedBudget { get; set; }
    }
}
