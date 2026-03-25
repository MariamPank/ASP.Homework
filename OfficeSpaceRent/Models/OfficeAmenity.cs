using OfficeSpaceRent.Common.Entity;

namespace OfficeSpaceRent.Models
{
    public class OfficeAmenity : Entity
    {
        public int OfficeSpaceId { get; set; }
        public OfficeSpace OfficeSpace { get; set; }

        public int AmenityId { get; set; }
        public Amenity Amenity { get; set; }

    }
}
