using _4Paws.Enums;

namespace _4Paws.DTOs.Owner.Responses
{
    public class OwnerAgreementShortResponse
    {
        public int Id { get; set; }
        public AgreementStatus Status { get; set; }
        public string PetName { get; set; }
        public string CareGiverName { get; set; }
    }
}
