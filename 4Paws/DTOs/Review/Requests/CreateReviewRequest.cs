using _4Paws.Enums;

namespace _4Paws.DTOs.Review.Requests
{
    public class CreateReviewRequest
    {
        public int AgreementId { get; set; }
        public Rating Rating { get; set; }
        public string? Comment { get; set; }

        // ── Target — only one should be provided ──────────────────────────
        public int? OwnerId { get; set; }
        public int? CareGiverId { get; set; }
        public int? PetId { get; set; }
    }
}
