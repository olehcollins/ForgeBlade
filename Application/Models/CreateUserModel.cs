using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public sealed class CreateUserModel
{
    [Required] [EmailAddress] public required string Email { get; set; }

    [Required]
    [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "Password must contain:at least 8 characters, at least 1 special characters, at least 1 number and up and down case characters.")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    public required string LastName { get; set; }

    [Phone]
    [Required(ErrorMessage = "Phone number is required.")]
    public required string PhoneNumber { get; set; }

}