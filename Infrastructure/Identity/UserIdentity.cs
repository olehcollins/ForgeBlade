using Infrastructure.DataTables;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class UserIdentity : IdentityUser<int>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Sex { get; set; }
    public required string Ethnicity { get; set; }
    public int Age { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; } = DateTime.Now;
    public DateTime LastModified { get; set; }

    // Navigation properties
    public virtual UserAddress? Address { get; set; }
    public virtual ICollection<UserEmergencyContact>? EmergencyContacts { get; set; }
    public virtual UserPhoto? Photo { get; set; }
    // Soft delete flag
    public bool isDeleted { get; set; } = false;

}