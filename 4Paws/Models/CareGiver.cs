using _4Paws.Common.Entity;
using _4Paws.Enums;

namespace _4Paws.Models
{
    public class CareGiver : Entity
    {
        public string CareGiverName { get; set; }
        public Rating CareGiverRating { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<Agreement> Agreements { get; set; } = new List<Agreement>();
    }
}
