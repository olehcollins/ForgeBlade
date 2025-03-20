using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;

namespace Infrastructure.DataTables;

public class UserPhoto
{
    [Key]
    public int Id { get; set; }
    public required string FilePath { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual required UserIdentity User { get; set; }
}