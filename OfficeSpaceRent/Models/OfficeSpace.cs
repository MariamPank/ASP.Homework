using OfficeSpaceRent.Common.Entity;
using System.ComponentModel.DataAnnotations;

namespace OfficeSpaceRent.Models
{
    public class OfficeSpace : Entity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Floor { get; set; }
        public double AreaSqm { get; set; }
        public decimal MonthlyRent { get; set; }
        public string OfficeNumber { get; set; } = string.Empty;
        public string Address { get; set; } = "Axis Towers, Tbilisi";
        public bool IsAvailable { get; set; } = true;
        public bool IsFurnished { get; set; }
        public int Capacity { get; set; }


        // OfficeImage => One to Many
        public List<OfficeImage> Images { get; set; } = new();

        //OfficeSpace => Many to Many
        public List<OfficeAmenity> OfficeAmenities { get; set; } = new();
        public List<RentalRequest> RentalRequests { get; set; } = new();
        public List<Lease> Leases { get; set; } = new();
    }
}
