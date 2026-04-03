using _4Paws.Enums;

namespace _4Paws.DTOs.Pet.Responses
{
    public class CreateListingResponse
    {
        public int Id { get; set; }
        public string PetName { get; set; }
        public Rating PetRating { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
    }
}
