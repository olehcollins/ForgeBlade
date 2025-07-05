using Infrastructure.DataTables;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using Infrastructure.Queries;

namespace Infrastructure.Persistence;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public class PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options)
    : IdentityDbContext<UserIdentity, UserRole, int>(options)
{
    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<UserEmergencyContact> UserEmergencyContacts { get; set; }
    public DbSet<TestUser> TestUsers { get; set; }
    private const int MaxLenght = 50;
    private const int EmailMaxLenght = 250;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(r => r.Name)
                .HasMaxLength(MaxLenght)
                .IsRequired();
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
            entity.Property(u => u.IsDeleted)
                .HasDefaultValue(false);
            // Ensure the Email property is required and unique.
            entity.Property(u => u.Email)
                .HasMaxLength(EmailMaxLenght)
                .IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();
            // Ensure the PhoneNumber property is required and unique.
            entity.Property(u => u.PhoneNumber)
                .HasMaxLength(MaxLenght)
                .IsRequired();
            entity.HasIndex(u => u.PhoneNumber).IsUnique();
            entity.Property(u => u.FirstName)
                .HasMaxLength(MaxLenght)
                .IsRequired();
            entity.Property(u => u.LastName)
                .HasMaxLength(MaxLenght)
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