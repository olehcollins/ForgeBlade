using Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterModel model);
}