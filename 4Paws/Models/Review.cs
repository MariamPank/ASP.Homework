using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class Review : Entity
    {
        public int AgreementId { get; set; }
        public Agreement Agreement { get; set; }

        public int ReviewerId { get; set; }
        public UserModel Reviewer { get; set; }

        public Rating Rating { get; set; }
        public string? Comment { get; set; }

        // ── Target — only one will be set ─────────────────────────────────
        public int? OwnerId { get; set; }
        public Owner? Owner { get; set; }

        public int? CareGiverId { get; set; }
        public CareGiver? CareGiver { get; set; }

        public int? PetId { get; set; }
        public Pet? Pet { get; set; }
    }
}
