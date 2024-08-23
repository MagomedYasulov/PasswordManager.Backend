using System.Security.Claims;

namespace PasswordManager.Backend.Services
{
    public interface ITokenService
    {
        public string GenerateAccessToken(ClaimsIdentity identity);

        public string GenerateRefreshToken();

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
    }
}
