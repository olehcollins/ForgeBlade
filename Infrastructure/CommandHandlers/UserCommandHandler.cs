using Infrastructure.Commands;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.CommandHandlers;

public class UserCommandHandler(UserManager<UserIdentity> userManager) :
    IRequestHandler<RegisterEmployeeCommand, IdentityResult>,
    IRequestHandler<AddUserToRoleCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
    {
        var user = new UserIdentity
        {
            UserName = request.Model.Email,
            Email = request.Model.Email,
            FirstName = request.Model.FirstName,
            LastName = request.Model.LastName,
            PhoneNumber = request.Model.PhoneNumber,
            DateOfBirth = request.Model.DateOfBirth,
            Sex = request.Model.Sex,
            Ethnicity = request.Model.Ethnicity,
        };

        var result = await userManager.CreateAsync(user, request.Model.Password);

        if (result.Succeeded)
        {
            result = await userManager.AddToRoleAsync(user, "Employee");
        }

        return result;
    }

    public async Task<IdentityResult> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken) => await userManager.AddToRoleAsync(request.User, request.Role);
}