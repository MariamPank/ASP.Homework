using System.ComponentModel.DataAnnotations;

namespace OfficeSpaceRent.DTOs.Requests
{
    public class CreateRentalRequest
    {
        public int OfficeSpaceId { get; set; }
        public DateTime DesiredStartDate { get; set; }
        public int DurationMonths { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
