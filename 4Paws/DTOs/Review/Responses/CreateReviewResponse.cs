using _4Paws.Enums;

namespace _4Paws.DTOs.Review.Responses
{
    public class CreateReviewResponse
    {
        public int Id { get; set; }
        public int AgreementId { get; set; }
        public string ReviewerName { get; set; }
        public Rating Rating { get; set; }
        public string RatingLabel { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // ── Target info ───────────────────────────────────────────────────
        public int? OwnerId { get; set; }
        public int? CareGiverId { get; set; }
        public int? PetId { get; set; }
    }
}
