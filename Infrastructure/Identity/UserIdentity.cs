using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataTables;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Identity;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public sealed class UserIdentity : IdentityUser<int>
{
    private const double Year = 365.2425;

    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    [MaxLength(50)]
    public required string Sex { get; init; }
    [MaxLength(50)]
    public required string Ethnicity { get; init; }
    [NotMapped]
    public int Age => (int)((DateTime.UtcNow - DateOfBirth).TotalDays / Year);
    public DateTimeOffset DateOfBirth { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTimeOffset LastModified { get; init; }

    // Navigation properties
    public UserAddress? Address { get; init; }
    public ICollection<UserEmergencyContact>? EmergencyContacts { get; init; }
    // Soft delete flag
    public bool IsDeleted { get; init; }
}