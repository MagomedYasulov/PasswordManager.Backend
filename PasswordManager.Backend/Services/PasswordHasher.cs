using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;

namespace PasswordManager.Backend.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, HashType.SHA384);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(providedPassword, hashedPassword, HashType.SHA384);
        }
    }
}
