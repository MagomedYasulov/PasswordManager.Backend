using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Backend.Data.Entities;

namespace PasswordManager.Backend.Data
{
    public class ApplicationContext : IdentityDbContext<User, IdentityRole<int>, int> //
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
    }
}
