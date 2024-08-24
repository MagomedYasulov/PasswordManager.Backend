using Microsoft.AspNetCore.Identity;

namespace PasswordManager.Backend.Data.Entities
{
    public class User : IdentityUser<int>, IEntity
    {
        public List<Token> RefreshTokens { get; set; } = [];
        public string Salt { get; set; } = string.Empty;
    }
}
