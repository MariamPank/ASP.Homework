using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class Owner : Entity
    {
        public string UserName { get; set; }
        public Rating OwnerRating { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; }

        public List<Pet> Pets { get; set; } = new();
        public List<Agreement> Agreements { get; set; } = new();
        public List<Listing> Listings { get; set; } = new();
        public List<Application> Applications { get; set; } = new();

    }
}
