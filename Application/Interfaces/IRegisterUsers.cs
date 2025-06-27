using Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces;

public interface IRegisterUsers
{
    Task<IdentityResult> RegisterEmployeeAsync(RegisterEmployeeModel model);
    Task<IdentityResult> RegisterAdminAsync(RegisterAdminModel model);
}