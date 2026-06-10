using _4Paws.Enums;

namespace _4Paws.DTOs.Caregiver.Responses
{
    public class CreateCaregiverProfileResponse
    {
        public int Id { get; set; }
        public Rating CaregiverRating { get; set; }
        public int UserId { get; set; }
    }
}
