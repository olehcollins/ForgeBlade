using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataTables;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public sealed class UserEmergencyContact
{
    [Key]
    public required int Id { get; init; }
    [MaxLength(50)]
    public required string FirstName { get; init; }
    [MaxLength(50)]
    public required string LastName { get; init; }
    [MaxLength(50)]
    public required string PhoneNumber { get; init; }
    [MaxLength(50)]
    public required string Relationship { get; init; }

    public int UserId { get; init; }
    [ForeignKey("UserId")]
    public required UserIdentity User { get; init; }
}