using Microsoft.IdentityModel.Tokens;
using PasswordManager.Backend.Extententions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PasswordManager.Backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateAccessToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            var lifeTime = _configuration.GetSection("JwtToken:LIFETIME").Get<int>();
            var jwt = new JwtSecurityToken
                (
                    issuer: _configuration["JwtToken:ISSUER"],
                    audience: _configuration["JwtToken:AUDIENCE"], //AUDIENCE
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(lifeTime)),
                    signingCredentials: _configuration.CreateSigningCredentials() // new SigningCredentials(_configuration.CreateSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64]; //32
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true, //you might want to validate the audience and issuer depending on your use case
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtToken:ISSUER"],
                    ValidAudience = _configuration["JwtToken:AUDIENCE"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _configuration.CreateSymmetricSecurityKey(),
                    ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception get principal from expired token");
                return null;
            }
        }
    }
}
