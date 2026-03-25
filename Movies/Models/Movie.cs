using Movies.Common.Entity;
using Movies.Enum;

namespace Movies.Models
{
    public class Movie : Entity
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }
        public Rating Rating { get; set; }
        public int ReleaseYear { get; set; }
    }
}
