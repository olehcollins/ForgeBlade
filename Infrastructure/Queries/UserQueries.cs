using System.Diagnostics.CodeAnalysis;
using Infrastructure.Identity;
using MediatR;

namespace Infrastructure.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetUserByIdQuery(string UserId) : IRequest<UserIdentity?>;
public sealed record GetUserByEmailQuery(string Email) : IRequest<UserIdentity?>;
public sealed record GetAllUsersQuery : IRequest<UserIdentity[]>;