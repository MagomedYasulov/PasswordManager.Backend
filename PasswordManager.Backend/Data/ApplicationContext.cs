using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Backend.Data.Entities;

namespace PasswordManager.Backend.Data
{
    public class ApplicationContext : DbContext 
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Token> Tokens => Set<Token>();
        public DbSet<Credential> Credentilals => Set<Credential>();

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Credential>()
                        .HasOne(c => c.User)
                        .WithMany(u => u.Credentials)
                        .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<User>().HasIndex(x => x.NormalizedLogin).IsUnique();
        }
    }
}
