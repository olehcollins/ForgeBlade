using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            // Ensure the Email property is required and unique.
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.HasIndex(u => u.Email).IsUnique();

            // Ensure the PhoneNumber property is required and unique.
            entity.Property(u => u.PhoneNumber)
                .IsRequired();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
        } );

        // Configure composite key for IdentityUserLogin<int>
        builder.Entity<IdentityUserLogin<int>>()
            .HasKey(l => new { l.LoginProvider, l.ProviderKey });

        // Configure composite key for IdentityUserRole<int>
        builder.Entity<IdentityUserRole<int>>()
            .HasKey(r => new { r.UserId, r.RoleId });

        // Configure composite key for IdentityUserToken<int>
        builder.Entity<IdentityUserToken<int>>()
            .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
    }
}