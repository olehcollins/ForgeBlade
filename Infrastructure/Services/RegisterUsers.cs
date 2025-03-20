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
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            Age = IdentityHelpers.CalculateAge(model.DateOfBirth),
            DateOfBirth = model.DateOfBirth,
            Sex = model.Sex,
            Ethnicity = model.Ethnicity,
        };

        await  userManager.AddToRoleAsync(user, model.Role);
        return await userManager.CreateAsync(user, model.Password);
    }

    public async Task<IdentityResult> RegisterAdminAsync(RegisterAdminModel model)
    {
        var user = new UserIdentity
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Sex = "Male",
            Ethnicity = "Black/African",
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (result.Succeeded) await userManager.AddToRoleAsync(user, "Admin");
        return result;
    }


}