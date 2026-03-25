using System.ComponentModel.DataAnnotations;

namespace OfficeSpaceRent.DTOs.Requests
{
    public class CreateOfficeRequest
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
        public List<string> ImageUrls { get; set; } = new();
        public List<int> AmenityIds { get; set; } = new();
    }
}
