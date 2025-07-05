using System.Diagnostics.CodeAnalysis;
using Application.Models;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Commands;

[ExcludeFromCodeCoverage(Justification = "Not part of Code Test")]
public sealed record RegisterEmployeeCommand(RegisterEmployeeModel Model) : IRequest<IdentityResult>;
public sealed record CreateUserCommand(CreateUserModel Model) : IRequest<IdentityResult>;
public sealed record AddUserToRoleCommand(UserIdentity User, string Role) : IRequest<IdentityResult>;