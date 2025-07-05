using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Identity;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public sealed class UserRole : IdentityRole<int>
{ }