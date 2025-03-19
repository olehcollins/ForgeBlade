using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<int>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int Age { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required string Role { get; set; }
    public required string Address { get; set; }
    public required string EmergencyContactName { get; set; }
    public required string EmergencyContactNumber { get; set; }
    public required string Relationship { get; set; }

}