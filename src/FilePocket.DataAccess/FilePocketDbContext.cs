using FilePocket.Domain.Entities;
using FilePocket.Domain.Entities.Consumption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.DataAccess;

public class FilePocketDbContext(DbContextOptions<FilePocketDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilePocketDbContext).Assembly);
    }

    public DbSet<FileMetadata> FilesMetadata { get; set; } // should be created inside pocket, or linked to default pocket
    
    public DbSet<Pocket> Pockets { get; set; }
    public DbSet<Folder> Folders { get; set; } // should be created inside pocket only
    public DbSet<SharedFile> SharedFiles { get; set; }

    public DbSet<AccountConsumption> AccountConsumptions { get; set; }

    private void SeedRolesWithAdminUser(ModelBuilder builder)
    {
        Guid adminUserId = Guid.NewGuid();
        Guid adminRoleId = Guid.NewGuid();

        builder.Entity<Role>().HasData
        (
            new Role
            {
                Name = "User",
                NormalizedName = "USER"
            },
            new Role
            {
                Id = adminRoleId,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            }
        );

        var hasher = new PasswordHasher<User>();

        builder.Entity<User>().HasData
        (
            new User
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com",
                EmailConfirmed = false,
                PasswordHash = hasher.HashPassword(null, "Admin123#"),
                SecurityStamp = string.Empty
            }
        );

        builder.Entity<IdentityUserRole<Guid>>().HasData
        (
            new IdentityUserRole<Guid>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            }
        );
    }
}
