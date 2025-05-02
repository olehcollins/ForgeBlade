using Infrastructure.DataTables;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Identity;

[ExcludeFromCodeCoverage]
public sealed class UserIdentity : IdentityUser<int>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Sex { get; init; }
    public required string Ethnicity { get; init; }
    public int Age { get; init; }
    public DateTime DateOfBirth { get; init; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime LastModified { get; init; }

    // Navigation properties
    public UserAddress? Address { get; init; }
    public ICollection<UserEmergencyContact>? EmergencyContacts { get; init; }
    public UserPhoto? Photo { get; init; }
    // Soft delete flag
    public bool IsDeleted { get; init; } = false;

}