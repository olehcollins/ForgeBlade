using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Utility;

[ExcludeFromCodeCoverage]
public static class IdentityHelpers
{
    public static int CalculateAge(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-age)) age--;

        return age;
    }
}