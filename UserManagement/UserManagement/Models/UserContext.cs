using Microsoft.EntityFrameworkCore;
namespace UserManagement.Models
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasIndex(u => u.Name)
                .IsUnique();
            modelBuilder.Entity<User>()
               .HasMany(u => u.Roles)
               .WithMany()
               .UsingEntity(j => j.ToTable("UserRoles")); // Join table for many-to-many relationship

            base.OnModelCreating(modelBuilder);
        }
    }
}
