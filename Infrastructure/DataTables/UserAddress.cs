using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataTables;

[ExcludeFromCodeCoverage]
public class UserAddress
{
    [Key]
    public required int Id { get; init; }
    public required string StreetName { get; init; }
    public required string PostalCode { get; init; }
    public required string City { get; init; }

    public int UserId { get; init; }
    [ForeignKey("UserId")]
    public virtual required UserIdentity User { get; init; }
}