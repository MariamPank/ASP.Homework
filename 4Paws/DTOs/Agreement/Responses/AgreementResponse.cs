using _4Paws.Enums;
using _4Paws.Models;

namespace _4Paws.DTOs.Agreement.Responses
{
    public class AgreementResponse
    {
        public int Id { get; set; }
        public AgreementStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AgreedFee { get; set; }
        public int OwnerId { get; set; }
        public int CareGiverId { get; set; }
        public int PetId { get; set; }
        public DateTime CompleteAt { get; set; }
    }
}
