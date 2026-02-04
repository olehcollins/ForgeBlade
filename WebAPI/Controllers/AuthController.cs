using Application.Models;
using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(
    UserManager<UserIdentity> userManager,
    SignInManager<UserIdentity> signInManager,
    IAccessTokenService tokenService
    ) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(ResponseModel<Dictionary<string,string?>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseModel<string>), StatusCodes.Status401Unauthorized)]
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
    public async Task<ActionResult<ResponseModel<string>>> CustomSignOut(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        await Task.WhenAll(
            userManager.RemoveAuthenticationTokenAsync(user!, "No Provider", "RefreshToken"),
            signInManager.SignOutAsync());

        return Ok(new ResponseModel<string>("Successfully sign out.", null));
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public ActionResult<ResponseModel<string>> ConfirmEmail()
        => Ok(new ResponseModel<string>("Email confirmed", null));
}