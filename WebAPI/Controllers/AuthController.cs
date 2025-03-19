using Application.Interfaces;
using Application.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth/")]
public class AuthController(IAuthService authService, UserManager<ApplicationUser> userManager) :
    ControllerBase
{
    [HttpPost("createUser")]
    public async Task<IActionResult> CreateUserAsync([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await authService.RegisterUserAsync(model);

        return result.Succeeded ? BadRequest() : Ok("User registered successfully");
    }

    [HttpGet("getAllUsers")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await userManager.Users.ToListAsync();

        return Ok(users);
    }
}