using _4Paws.Enums;

namespace _4Paws.DTOs.Caregiver.Responses
{
    public class GetCaregiverAgreementsResponse
    {
        public int Id { get; set; }
        public AgreementStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AgreedFee { get; set; }
        public string OwnerName { get; set; }
        public string PetName { get; set; }
    }
}
