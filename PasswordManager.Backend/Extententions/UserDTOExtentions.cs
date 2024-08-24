using Newtonsoft.Json;
using PasswordManager.Backend.DTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace PasswordManager.Backend.Extententions
{
    /// <summary>
    /// Создание  идентификационных данных пользователя
    /// </summary>
    /// <param name="serializeSettings">Настройки сериализации</param>
    /// <returns></returns>
    public static class UserDTOExtentions
    {
        public static ClaimsIdentity GetIdentity(this UserDTO userDTO, JsonSerializerSettings serializeSettings)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, userDTO.Login),
                new Claim(ClaimTypes.Name, userDTO.Login),
                new Claim(ClaimTypes.NameIdentifier, userDTO.Id.ToString()),
                new Claim("salt", userDTO.Salt),
                new Claim("user", JsonConvert.SerializeObject(userDTO, serializeSettings), JsonClaimValueTypes.Json)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
