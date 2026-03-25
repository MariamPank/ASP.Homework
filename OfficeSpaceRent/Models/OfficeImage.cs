using OfficeSpaceRent.Common.Entity;

namespace OfficeSpaceRent.Models
{
    public class OfficeImage : Entity
    {
        public string ImageUrl { get; set; } = string.Empty;

        // OfficeSpace => One to Many
        public int OfficeSpaceId { get; set; }
        public OfficeSpace OfficeSpace { get; set; }
    }
}
