namespace _4Paws.DTOs.Auth.Requests
{
    public class VerifyEmailRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
