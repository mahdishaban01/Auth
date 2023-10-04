using Common.Entities;

namespace Common.DTOs
{
    public class AuthenticationResponseDTO
    {
        public bool IsAuthSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
        public User User { get; set; }
    }
}
