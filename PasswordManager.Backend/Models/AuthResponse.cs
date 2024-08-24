using PasswordManager.Backend.DTOs;

namespace PasswordManager.Backend.Models
{
    public class AuthResponse
    {
        public UserDTO? User { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
    }
}
