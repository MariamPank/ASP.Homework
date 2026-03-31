using _4Paws.Enums;

namespace _4Paws.DTOs.Owner.Responses
{
    public class CreateOwnerProfileResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Rating OwnerRating { get; set; }
        public int UserId { get; set; }
    }
}
