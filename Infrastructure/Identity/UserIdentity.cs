using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataTables;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Identity;

[ExcludeFromCodeCoverage]
public sealed class UserIdentity : IdentityUser<int>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50)]
    public required string Sex { get; init; }
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50)]
    public required string Ethnicity { get; init; }
    public DateTime DateOfBirth { get; init; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime LastModified { get; init; }

    // Navigation properties
    public UserAddress? Address { get; init; }
    public ICollection<UserEmergencyContact>? EmergencyContacts { get; init; }
    public UserPhoto? Photo { get; init; }

    [NotMapped]
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
    // Soft delete flag
    public bool IsDeleted { get; init; } = false;

}