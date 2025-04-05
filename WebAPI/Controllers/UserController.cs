using System.Collections.Immutable;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IRegisterUsers registerUsers, UserManager<UserIdentity> userManager,
    SignInManager<UserIdentity> signInManager) :
    ControllerBase
{
    [HttpPost("signin")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] LoginModel model)
    {
        // 1. Find the user by email.
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("User does not exist");
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
            return Unauthorized("Invalid credentials");
        }

        // 3. Generate a JWT token for the authenticated user.
        //var token = tokenService.GenerateToken(user);

        // 4. Return the token.
        //return Ok(new { Token = token });
        return Ok();
    }

    [HttpPost("register/employee")]
    public async Task<IActionResult> CreateEmployeeAsync([FromBody] RegisterEmployeeModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await registerUsers.RegisterEmployeeAsync(model);

        return result.Succeeded ? BadRequest() : Ok("User registered successfully");
    }

    [AllowAnonymous]
    [HttpPost("register/admin")]
    public async Task<IActionResult> CreateAdminAsync([FromBody] RegisterAdminModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await registerUsers.RegisterAdminAsync(model);

        return result.Succeeded ?
            Ok($"User {model.Email} registered successfully")
            : ValidationProblem(
                extensions: new Dictionary<string, object?>
                    {
                        { "details", result.Errors.Select(e => e.Description).ToArray() }
                    });
    }

    [AllowAnonymous]
    [HttpGet("allusers")]
    public async Task<IActionResult> GetAllEmployeesAsync()
    {
        var users = await userManager.Users.ToListAsync();

        return Ok(users);
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserProfile(int userId)
    {
        var user = await userManager.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpGet("2user/{id:int}")]
    public async Task<IActionResult> GetUser(int id, [FromServices] IMemoryCache cache)
    {
        string cacheKey = $"user_{id}";
        if (cache.TryGetValue(cacheKey, out UserIdentity? user)) return Ok(user);

        user = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        cache.Set(cacheKey, user, cacheOptions);
        return Ok(user);
    }
}