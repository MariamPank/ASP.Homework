namespace OfficeSpaceRent.DTOs.Responses
{
    public class OfficeResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Floor { get; set; }
        public double AreaSqm { get; set; }
        public decimal MonthlyRent { get; set; }
        public string OfficeNumber { get; set; }
        public string Address { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsFurnished { get; set; }
        public int Capacity { get; set; }

        public List<string> Images { get; set; } = new();
        public List<string> Amenities { get; set; } = new();
    }
}
