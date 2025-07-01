using System.Globalization;
using Application.Models;
using Infrastructure.Commands;
using Infrastructure.Identity;
using Infrastructure.Queries;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserManager<UserIdentity> userManager,
    SignInManager<UserIdentity> signInManager, IAccessTokenService tokenService, ISender mediatorSender) :
    ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new ResponseModel<string>("User non existent", null));
        }

        var result = await signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            isPersistent: false,
            lockoutOnFailure: false
            );

        if (!result.Succeeded)
        {
            return Unauthorized(new ResponseModel<string>("Invalid login credentials", null));
        }

        var newTokens = await tokenService.GenerateTokens(user);

        return Ok(
            new ResponseModel<Dictionary<string, string?>>("Successfully sign in.",
                newTokens));
    }

    [HttpGet("sign-out/{userId}")]
    public async Task<IActionResult> CustomSignOut(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        await Task.WhenAll(
            userManager.RemoveAuthenticationTokenAsync(user!, "No Provider", "RefreshToken"),
            signInManager.SignOutAsync());

        return Ok(new ResponseModel<string>("Successfully sign out.", null));
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public IActionResult ConfirmEmail()
        => Ok(new ResponseModel<string>("Email confirmed", null));

    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllEmployeesAsync()
        => Ok(new ResponseModel<UserIdentity[]>(
            $"Users found",
            await mediatorSender.Send(new GetAllUsersQuery()))
        );

    [HttpGet("search-users")]
    public async Task<IActionResult> GetAllTestUsersAsync(string? queryTerm, string? sortColumn, string? sortOrder, int? pageNumber , int? pageSize)
    {
        var data = await mediatorSender.Send(new FindAllTestUsersQuery(
            queryTerm?.Trim(),
            sortColumn?.Trim().ToLower(CultureInfo.InvariantCulture),
            sortOrder?.Trim().ToLower(CultureInfo.InvariantCulture),
            pageNumber is null || pageNumber.Value < 1 ? 1 : pageNumber.Value,
            pageSize is null || pageSize.Value < 10 ? 10 : pageSize.Value));

        return data.Length > 0
            ? Ok(new ResponseModel<TestUser[]>(
                $"Test users found", data))
            : NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await mediatorSender.Send(new GetUserByIdQuery(userId));

        return user is null
            ? NotFound(new ResponseModel<UserIdentity>($"User {userId} not found", null))
            : Ok(new ResponseModel<UserIdentity>($"User {userId} found", user));
    }

    [HttpGet("user/by-email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await mediatorSender.Send(new GetUserByEmailQuery(email));

        return user is null
            ? NotFound(new ResponseModel<UserIdentity>($"User {email} not found", null))
            : Ok(new ResponseModel<UserIdentity>($"User {email} found", user));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register/employee")]
    public async Task<IActionResult> CreateEmployeeAsync([FromBody] RegisterEmployeeModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await mediatorSender.Send(new RegisterEmployeeCommand(model));

        return result.Succeeded
            ? Ok(new ResponseModel<string>($"User registered successfully", model.Email))
            : ValidationProblem(
                extensions: new Dictionary<string, object?>
                {
                    { "details", result.Errors.Select(e => e.Description).ToArray() }
                });
    }
}