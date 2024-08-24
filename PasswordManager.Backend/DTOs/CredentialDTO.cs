namespace PasswordManager.Backend.DTOs
{
    public class CredentialDTO : BaseDTO
    {

        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } =string.Empty;
        public string? Service { get; set; }
        public string? URL { get; set; }
    }
}
