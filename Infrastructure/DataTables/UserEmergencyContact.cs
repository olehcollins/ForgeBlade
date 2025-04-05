using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;

namespace Infrastructure.DataTables;

public class UserEmergencyContact
{
    [Key]
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Relationship { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual required UserIdentity User { get; set; }
}