using System.Diagnostics;
using System.Text;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Utility;

namespace Infrastructure.Services;

public sealed class RegisterUsers(UserManager<UserIdentity> userManager) : IRegisterUsers
{
    public async Task<IdentityResult> RegisterEmployeeAsync(RegisterEmployeeModel model)
    {
        var user = new UserIdentity
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            Age = IdentityHelpers.CalculateAge(model.DateOfBirth),
            DateOfBirth = model.DateOfBirth,
            Sex = model.Sex,
            Ethnicity = model.Ethnicity,
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded) result = await userManager.AddToRoleAsync(user, "Employee");

        // Debug-time Assertion
        Debug.Assert(result is IdentityResult, "result is not of type IdentityResult");
        Debug.Assert(result != null, "result is null");
        return result;
    }

    public async Task<IdentityResult> RegisterAdminAsync(RegisterAdminModel model)
    {
        var user = new UserIdentity
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            Sex = "Male",
            Ethnicity = "Black/African"
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded) result = await userManager.AddToRoleAsync(user, "Admin");

        // Debug-time Assertion
        Debug.Assert(result is IdentityResult, "result is not of type IdentityResult");
        Debug.Assert(result != null, "result is null");

        return result;
    }
}