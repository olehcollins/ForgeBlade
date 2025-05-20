using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataTables;

[ExcludeFromCodeCoverage]
public sealed class UserPhoto
{
    [Key]
    public int Id { get; init; }
    public required string FilePath { get; init; }
    public DateTime UploadedAt { get; init; } = DateTime.Now;

    public int UserId { get; init; }
    [ForeignKey("UserId")]
    public required UserIdentity User { get; init; }
}