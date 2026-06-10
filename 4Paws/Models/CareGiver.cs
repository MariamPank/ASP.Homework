using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class CareGiver : Entity
    {
        public Rating CareGiverRating { get; set; }
        public string Bio { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; }

        public List<Agreement> Agreements { get; set; } = new();
        public List<Listing> Listings { get; set; } = new();
        public List<Application> Applications { get; set; } = new();
    }
}
