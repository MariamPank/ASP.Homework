using OfficeSpaceRent.Common.Entity;

namespace OfficeSpaceRent.Models
{
    public class Amenity : Entity
    {
        public string Name { get; set; } = string.Empty;


        //OfficeSpace => Many to Many

        public List<OfficeAmenity> OfficeAmenities { get; set; } = new();
    }
}
