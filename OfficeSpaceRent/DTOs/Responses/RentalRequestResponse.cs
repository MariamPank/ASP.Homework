namespace OfficeSpaceRent.DTOs.Responses
{
    public class RentalRequestResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public int OfficeSpaceId { get; set; }
        public string OfficeTitle { get; set; }
        public DateTime DesiredStartDate { get; set; }
        public int DurationMonths { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
