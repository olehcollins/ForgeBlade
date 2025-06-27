using Infrastructure.DataTables;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using Infrastructure.Queries;

namespace Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<UserIdentity, UserRole, int>(options)
{
    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<UserEmergencyContact> UserEmergencyContacts { get; set; }
    public DbSet<UserPhoto> UserPhotos { get; set; }
    public DbSet<TestUser> TestUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(r => r.Name)
                .HasColumnType("nvarchar(50)")
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(r => r.Name).IsUnique();
        });

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
            entity.Property(u => u.IsDeleted)
                .HasDefaultValue(false);
            // Ensure the Email property is required and unique.
            entity.Property(u => u.Email)
                .HasColumnType("nvarchar(256)")
                .IsRequired()
                .HasMaxLength(256);
            entity.HasIndex(u => u.Email).IsUnique();
            // Ensure the PhoneNumber property is required and unique.
            entity.Property(u => u.PhoneNumber)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
            entity.Property(u => u.FirstName)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(u => u.LastName)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50)
                .IsRequired();
            entity.HasQueryFilter(u => !u.IsDeleted);
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