using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataTables;

[ExcludeFromCodeCoverage]
public class UserEmergencyContact
{
    [Key]
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Relationship { get; init; }

    public int UserId { get; init; }
    [ForeignKey("UserId")]
    public virtual required UserIdentity User { get; init; }
}