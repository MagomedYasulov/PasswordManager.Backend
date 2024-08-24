using Microsoft.AspNetCore.Identity;

namespace PasswordManager.Backend.Data.Entities
{
    public class User : BaseEntity
    {
        public string Login { get; set; } = string.Empty;
        public string NormalizedLogin { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;

        public List<Token> RefreshTokens { get; set; } = [];
        public List<Credential> Credentials { get; set; } = [];

    }
}
