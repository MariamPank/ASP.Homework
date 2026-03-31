using _4Paws.Enums;

namespace _4Paws.DTOs.Caregiver.Responses
{
    public class CaregiverAgreementShortResponse
    {
        public int Id { get; set; }
        public AgreementStatus Status { get; set; }
        public string PetName { get; set; }
        public string OwnerName { get; set; }
    }
}
