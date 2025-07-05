using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.DataTables;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public sealed class UserPhoto
{
    private const int StringLength = 100;

    [Key]
    public int Id { get; init; }
    [MaxLength(StringLength, ErrorMessage = "FilePath cannot exceed 200 characters.")]
    public required string FilePath { get; init; }
    public DateTimeOffset UploadedAt { get; init; } = DateTime.UtcNow;

    public int UserId { get; init; }
    [ForeignKey("UserId")]
    public required UserIdentity User { get; init; }
}