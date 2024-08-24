using Microsoft.AspNetCore.Identity;

namespace PasswordManager.Backend.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
