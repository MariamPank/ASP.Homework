using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class Owner : Entity
    {
        public string UserName { get; set; }
        public Rating OwnerRating { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<Pet> Pets { get; set; } = new List<Pet>();
        public List<Agreement> Agreements { get; set; } = new List<Agreement>();

    }
}
