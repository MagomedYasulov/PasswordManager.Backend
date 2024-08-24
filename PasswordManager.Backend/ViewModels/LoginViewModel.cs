namespace PasswordManager.Backend.ViewModels
{
    public class LoginViewModel
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;

        public string NormalizedLogin => Login.ToUpper();
    }
}
