namespace PasswordManager.Backend.Data.Entities
{
    public class Credential : BaseEntity
    {
        public string Password { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string? URL { get; set; } 
        public string? Service { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
