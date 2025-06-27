using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataTables;

[ExcludeFromCodeCoverage]
public sealed class UserPhoto
{
    private const int StringLenght = 100;
    [Key]
    public int Id { get; init; }
    [StringLength(StringLenght, ErrorMessage = "FilePath cannot exceed 200 characters.")]
    public required string FilePath { get; init; }
    public DateTime UploadedAt { get; init; } = DateTime.Now;

    public int UserId { get; init; }
    [ForeignKey("UserId")]
    public required UserIdentity User { get; init; }
}