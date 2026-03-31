using _4Paws.Enums;

namespace _4Paws.DTOs.Owner.Requests
{
    public class CreateOwnerProfileRequest
    {
        public string UserName { get; set; }
        public string? Bio { get; set; }
    }
}
