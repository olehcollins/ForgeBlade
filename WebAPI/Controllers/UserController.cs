using System.Globalization;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IRegisterUsers registerUsers, UserManager<UserIdentity> userManager,
    SignInManager<UserIdentity> signInManager, IAccessTokenService tokenService) :
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

        if (DateTime.ParseExact(refreshTokenExpiration ?? "no token", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) < DateTime.Now)
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

    [HttpGet("users")]
    public async Task<IActionResult> GetAllEmployeesAsync()
    {
        var users = await userManager.Users.ToArrayAsync();

        return Ok(users);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserProfile(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user is null
            ? NotFound(new ResponseModel<UserIdentity>($"User {userId} not found", null))
            : Ok(new ResponseModel<UserIdentity>($"User {userId} found", user));
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