using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataTables;

[ExcludeFromCodeCoverage]
public sealed class UserEmergencyContact
{
    [Key]
    public required int Id { get; init; }
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50)]
    public required string FirstName { get; init; }
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50)]
    public required string LastName { get; init; }
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50)]
    public required string PhoneNumber { get; init; }
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50)]
    public required string Relationship { get; init; }

    public int UserId { get; init; }
    [ForeignKey("UserId")]
    public required UserIdentity User { get; init; }
}