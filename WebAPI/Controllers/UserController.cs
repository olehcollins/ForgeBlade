using System.ComponentModel.DataAnnotations;
using Application.Models;
using Infrastructure.Commands;
using Infrastructure.Identity;
using Infrastructure.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ISender mediatorSender) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await mediatorSender.Send(new CreateUserCommand(model));

        return result.Succeeded
            ? Ok(new ResponseModel<string>($"User created successfully", model.Email))
            : ValidationProblem(
                extensions: new Dictionary<string, object?>
                {
                    { "details", result.Errors.Select(e => e.Description).ToArray() }
                });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register/employee")]
    public async Task<IActionResult> CreateEmployeeAsync([FromBody] RegisterEmployeeModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await mediatorSender.Send(new RegisterEmployeeCommand(model));

        return result.Succeeded
            ? Ok(new ResponseModel<string>($"User registered successfully", model.Email))
            : ValidationProblem(
                extensions: new Dictionary<string, object?>
                {
                    { "details", result.Errors.Select(e => e.Description).ToArray() }
                });
    }

    [ProducesResponseType(typeof(ResponseModel<UserIdentity>), StatusCodes.Status404NotFound)]
    [HttpGet("find-user/by-email/{email}")]
    public async Task<ActionResult<ResponseModel<UserIdentity>>> GetUserByEmail(string email)
    {
        var user = await mediatorSender.Send(new GetUserByEmailQuery(email));

        return user is null
            ? NotFound(new ResponseModel<UserIdentity>($"User {email} not found", null))
            : Ok(new ResponseModel<UserIdentity>($"User {email} found", user));
    }

    [ProducesResponseType(typeof(ResponseModel<UserIdentity>), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [HttpGet("find-user/by-id/{userId}")]
    public async Task<ActionResult<ResponseModel<UserIdentity>>> GetUserById(string userId)
    {
        var user = await mediatorSender.Send(new GetUserByIdQuery(userId));

        return user is null
            ? NotFound(new ResponseModel<UserIdentity>($"User {userId} not found", null))
            : Ok(new ResponseModel<UserIdentity>($"User {userId} found", user));
    }

    [HttpGet("all-users")]
    public async Task<ActionResult<ResponseModel<UserIdentity[]>>> GetAllEmployeesAsync()
        => Ok(new ResponseModel<UserIdentity[]>(
            $"Users found",
            await mediatorSender.Send(new GetAllUsersQuery()))
        );

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpGet("search-test-users")]
    public async Task<ActionResult<ResponseModel<TestUser[]>>> SearchTestUsersAsync(
        string? queryTerm,
        string? sortColumn,
        string? sortOrder,
        [FromQuery, Range(1, int.MaxValue)] int? pageNumber,
        [FromQuery, Range(1, int.MaxValue)] int? pageSize)
    {
        var data = await mediatorSender.Send(new FindAllTestUsersQuery(
            queryTerm?.Trim(),
            sortColumn?.Trim().ToUpperInvariant(),
            sortOrder?.Trim().ToUpperInvariant(),
            pageNumber ?? 1,
            pageSize ?? 10));

        return data.Length > 0
            ? Ok(new ResponseModel<TestUser[]>(
                $"Test users found", data))
            : NoContent();
    }
}