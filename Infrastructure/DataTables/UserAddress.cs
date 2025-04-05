using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;

namespace Infrastructure.DataTables;

public class UserAddress
{
    [Key]
    public required int Id { get; set; }
    public required string StreetName { get; set; }
    public required string PostalCode { get; set; }
    public required string City { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual required UserIdentity User { get; set; }
}