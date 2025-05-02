using System.Globalization;
using Application.Interfaces;
using Application.Models;
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
public class UserController(IRegisterUsers registerUsers, UserManager<UserIdentity> userManager,
    SignInManager<UserIdentity> signInManager, IAccessTokenService tokenService, ISender mediatorSender) :
    ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] LoginModel model)
    {
        // 1. Find the user by email.
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new ResponseModel<string>("User non existent", null));
        }

        // 2. Validate credentials.
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
    [HttpGet("refresh/{userId}")]
    public async Task<IActionResult> Refresh(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if ( user is null) return Unauthorized(new ResponseModel<string>("User non existent", null));

        var refreshTokenExpiration = await userManager.GetAuthenticationTokenAsync(user, "No Provider", "RefreshToken");

        if (refreshTokenExpiration is null || DateTime.ParseExact(refreshTokenExpiration, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < DateTime.Now)
        {
            var tokens = new Dictionary<string, string?>
            {
                { "access_token", null },
                { "refresh_token_expiration", null }
            };
            return Ok(
                new ResponseModel<Dictionary<string, string?>>("Tokens expired. Please sign in again.",
                    tokens));
        }
        var newTokens = await tokenService.GenerateTokens(user);

        return Ok(
            new ResponseModel<Dictionary<string, string?>>("New access and refresh tokens generated",
                newTokens));
    }

    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllEmployeesAsync()
        => Ok(new ResponseModel<UserIdentity[]>(
            $"Users found",
            await mediatorSender.Send(new GetAllUsersQuery()))
        );

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


    // [HttpPost("register/admin")]
    // public async Task<IActionResult> CreateAdminAsync([FromBody] RegisterAdminModel model)
    // {
    //     if (!ModelState.IsValid) return BadRequest(ModelState);
    //
    //     var result = await registerUsers.RegisterAdminAsync(model);
    //
    //     return result.Succeeded
    //         ? Ok(new ResponseModel<UserIdentity>($"User {model.Email} registered successfully", null))
    //         : ValidationProblem(
    //             extensions: new Dictionary<string, object?>
    //             {
    //                 { "details", result.Errors.Select(e => e.Description).ToArray() }
    //             });
    // }
}