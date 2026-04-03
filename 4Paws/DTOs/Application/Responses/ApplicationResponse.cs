using _4Paws.Enums;

namespace _4Paws.DTOs.Application.Responses
{
    public class ApplicationResponse
    {
        public int Id { get; set; }
        public int ListingId { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public string Message { get; set; }
        public ApplicationStatus Status { get; set; } // Pending, Accepted, Rejected
        public DateTime CreatedAt { get; set; }
    }
}
