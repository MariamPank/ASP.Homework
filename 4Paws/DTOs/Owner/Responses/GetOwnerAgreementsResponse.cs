using _4Paws.Enums;

namespace _4Paws.DTOs.Owner.Responses
{
    public class GetOwnerAgreementsResponse
    {
        public int Id { get; set; }
        public AgreementStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AgreedFee { get; set; }
        public string CareGiverName { get; set; }
        public string PetName { get; set; }
    }
}
