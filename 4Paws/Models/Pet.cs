using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class Pet :  Entity
    {
        public string PetName { get; set; }
        public Rating PetRating { get; set; }
        public string Description { get; set; }


        public int OwnerId { get; set; }
        public Owner Owner { get; set; }

        public List<Agreement> Agreements { get; set; } = new List<Agreement>();
    }
}
