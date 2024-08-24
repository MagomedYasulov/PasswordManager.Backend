namespace PasswordManager.Backend.DTOs
{
    public class UserDTO : BaseDTO
    {
        public string Login { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string? Email { get; set; } 
    }
}
