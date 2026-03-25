namespace OfficeSpaceRent.DTOs.Responses
{
    public class LeaseResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public int OfficeSpaceId { get; set; }
        public string OfficeTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public string Status { get; set; }
    }
}
