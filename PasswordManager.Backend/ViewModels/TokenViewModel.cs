namespace PasswordManager.Backend.ViewModels
{
    public class TokenViewModel
    {
        public string AccessToken { get; set; } =string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
