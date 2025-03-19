using Application.Interfaces;
using Application.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Utility;

namespace Infrastructure.Services;

public sealed class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
{
    public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            Role = model.Role,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            Age = IdentityHelpers.CalculateAge(model.DateOfBirth),
            DateOfBirth = model.DateOfBirth,
            Address = model.Address,
            EmergencyContactName = model.EmergencyContactName,
            EmergencyContactNumber = model.EmergencyContactNumber,
            Relationship = model.Relationship,
        };

        await  userManager.AddToRoleAsync(user, model.Role);
        return await userManager.CreateAsync(user, model.Password);
    }
}