using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Models;

public sealed class RegisterEmployeeModel
{
    [Required(ErrorMessage = "Role is required.")]
    [RegularExpression(@"^(Admin|Owner|Manager|Worker)$", ErrorMessage = "Invalid role. Allowed roles: Admin, Owner, Manager, Worker.")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is required.")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    [ValidName]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    [ValidName]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of Birth is required.")]
    [PastDate(ErrorMessage = "Date of Birth must be a valid past date.")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    [RegularExpression(@"^(Male|Female)$", ErrorMessage = "Invalid sex. Allowed sexes: Male, Female.")]
    public string Sex { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ethnicity is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Value must be between 2 and 50 characters.")]
    [ValidName]
    public string Ethnicity { get; set; } = string.Empty;

    [Required(ErrorMessage = "A name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    [ValidName]
    public string EmergencyContactFirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "A name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    [ValidName]
    public string EmergencyContactLastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone]
    public string EmergencyContactNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Relationship is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Relationship must be between 2 and 50 characters.")]
    [ValidName]
    public string Relationship { get; set; } = string.Empty;
}

/// <summary>
/// Custom validation attribute to ensure names contain only letters and spaces.
/// </summary>
public sealed class ValidNameAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string name && !Regex.IsMatch(name, @"^[A-Za-z]+(?: [A-Za-z]+)*$"))
        {
            return new ValidationResult("Name must only contain letters and spaces.");
        }
        return ValidationResult.Success!;
    }
}

/// <summary>
/// Custom validation attribute to ensure Date of Birth is in the past.
/// </summary>
public sealed class PastDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date && date >= DateTime.Today)
        {
            return new ValidationResult("Date of Birth must be a valid past date.");
        }
        return ValidationResult.Success!;
    }
}