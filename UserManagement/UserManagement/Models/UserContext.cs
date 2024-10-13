using Microsoft.EntityFrameworkCore;
using UserManagement.Services;
namespace UserManagement.Models
{
    public class UserContext:DbContext
    {
        public UserContext() : base(new DbContextOptions<UserContext>()) { }

        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
        }
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<RoleModel> Roles { get; set; } = null!;
        public DbSet<UserRoleModel> UserRoleModels { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<RoleModel>().HasData(
                new RoleModel {Id=1, Name = "Admin", Description = "Administrator RoleModel", IsActive = true },
                new RoleModel {Id=2, Name = "UserModel", Description = "Regular UserModel RoleModel", IsActive = true }
            );
            modelBuilder.Entity<UserModel>().HasData(
                new UserModel {Id=1, Username = "Default UserModel", Password = EncryptorDecryptor.HashPassword("UserModel"), Email = "UserModel@example.com",IsActive=true },
                new UserModel {Id=2, Username = "Default Admin", Password = EncryptorDecryptor.HashPassword("Admin"), Email = "admin@example.com",IsActive = true }
            );
            modelBuilder.Entity<UserRoleModel>().HasData(
            new UserRoleModel { UserId = 1, RoleId = 2 }, 
            new UserRoleModel { UserId = 2, RoleId = 1 }, 
            new UserRoleModel { UserId = 2, RoleId = 2 }  
            );
            modelBuilder.Entity<RoleModel>()
                .HasIndex(u => u.Name)
                .IsUnique();
            modelBuilder.Entity<RoleModel>()
            .HasIndex(r => r.Description)
            .IsUnique();
            modelBuilder.Entity<UserRoleModel>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        }
    }

}

