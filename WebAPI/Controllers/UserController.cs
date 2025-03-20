using System.Collections.Immutable;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IRegisterUsers registerUsers, UserManager<UserIdentity> userManager) :
    ControllerBase
{
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
        // if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var result = await registerUsers.RegisterAdminAsync(model);

        Console.WriteLine(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));

        var details = new Dictionary<string, object?>
        {
            { "details", result.Errors.Select(e => e.Description).ToArray() }
        };

        return result.Succeeded ?
            Ok("User registered successfully")
            : ValidationProblem(extensions: details);
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