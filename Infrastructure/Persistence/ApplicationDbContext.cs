using Infrastructure.DataTables;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<UserIdentity, UserRole, int>(options)
{
    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<UserEmergencyContact> UserEmergencyContacts { get; set; }
    public DbSet<UserPhoto> UserPhotos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserRole>().ToTable("Roles");
        builder.Entity<UserIdentity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasOne(a => a.Address)
                .WithOne(a  => a.User)
                .HasForeignKey<UserAddress>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(a => a.EmergencyContacts)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(a => a.Photo)
                .WithOne(u => u.User)
                .HasForeignKey<UserPhoto>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(u => u.isDeleted)
                .HasDefaultValue(false);
            // Ensure the Email property is required and unique.
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            entity.HasIndex(u => u.Email).IsUnique();
            // Ensure the PhoneNumber property is required and unique.
            entity.Property(u => u.PhoneNumber)
                .IsRequired();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
            entity.Property(u => u.FirstName)
                .IsRequired();
            entity.Property(u => u.LastName)
                .IsRequired();
            entity.HasQueryFilter(u => !u.isDeleted);
        } );

        // Configure the composite key for IdentityUserLogin<int>
        builder.Entity<IdentityUserLogin<int>>()
            .HasKey(l => new { l.LoginProvider, l.ProviderKey });

        // Configure the composite key for IdentityUserRole<int>
        builder.Entity<IdentityUserRole<int>>()
            .HasKey(r => new { r.UserId, r.RoleId });

        // Configure the composite key for IdentityUserToken<int>
        builder.Entity<IdentityUserToken<int>>()
            .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
    }
}