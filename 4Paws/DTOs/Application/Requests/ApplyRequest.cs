using _4Paws.Enums;

namespace _4Paws.DTOs.Application.Requests
{
    public class ApplyRequest
    {
        public int ListingId { get; set; }
        public AppliedBy AppliedBy { get; set; }
        public string Message { get; set; } // "Hi, I'd love to watch your cat!"
        public decimal? PropossedFee { get; set; }
    }
}
