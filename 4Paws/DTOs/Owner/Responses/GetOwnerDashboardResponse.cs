using _4Paws.Enums;

namespace _4Paws.DTOs.Owner.Responses
{
    public class GetOwnerDashboardResponse
    {
        public int OwnerId { get; set; }
        public string UserName { get; set; }
        public Rating OwnerRating { get; set; }

        public int TotalPets { get; set; }
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }

        public int TotalAgreements { get; set; }
        public int ActiveAgreements { get; set; }
        public int CompletedAgreements { get; set; }

        public List<OwnerListingShortResponse> RecentListings { get; set; } = new();
        public List<OwnerAgreementShortResponse> RecentAgreements { get; set; } = new();
    }
}
