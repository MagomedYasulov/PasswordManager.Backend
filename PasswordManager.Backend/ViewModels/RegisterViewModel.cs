namespace PasswordManager.Backend.ViewModels
{
    public class RegisterViewModel
    {
        public string Login { get; set; } = string.Empty;
        public string NormalizedLogin => Login.ToUpper();
        public string Password { get; set; } = string.Empty;
    }
}
