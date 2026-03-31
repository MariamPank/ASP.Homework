using _4Paws.Enums;

namespace _4Paws.DTOs.Caregiver.Responses
{
    public class GetCaregiverByIdResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Rating CaregiverRating { get; set; }
        public int UserId { get; set; }
    }
}
