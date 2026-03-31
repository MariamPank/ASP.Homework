using _4Paws.DTOs.Owner.Responses;
using _4Paws.Enums;

namespace _4Paws.DTOs.Caregiver.Responses
{
    public class GetCaregiverDashboardResponse
    {
        public int CaregiverId { get; set; }
        public string UserName { get; set; }
        public Rating CaregiverRating { get; set; }
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }

        public int TotalAgreements { get; set; }
        public int ActiveAgreements { get; set; }
        public int CompletedAgreements { get; set; }

        public List<CaregiverListingShortResponse> RecentListings { get; set; } = new();
        public List<CaregiverAgreementShortResponse> RecentAgreements { get; set; } = new();
    }
}
