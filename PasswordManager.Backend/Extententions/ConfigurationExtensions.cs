using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PasswordManager.Backend.Extententions
{
    public static class ConfigurationExtensions
    {
        public static SigningCredentials CreateSigningCredentials(this IConfiguration configuration)
        {
            return new SigningCredentials(
                configuration.CreateSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            );
        }

        public static SymmetricSecurityKey CreateSymmetricSecurityKey(this IConfiguration configuration)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtToken:KEY"]!));
        }
    }
}
