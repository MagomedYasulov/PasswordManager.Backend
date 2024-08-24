namespace PasswordManager.Backend.ViewModels
{
    public class CredentialViewModel
    {
        public string Password { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string? Service { get; set; }
        public string? URL { get; set; }
    }
}
