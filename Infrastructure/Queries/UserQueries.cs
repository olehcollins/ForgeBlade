using System.Diagnostics.CodeAnalysis;
using Infrastructure.Identity;
using MediatR;

namespace Infrastructure.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetUserByIdQuery(string UserId) : IRequest<UserIdentity?>;
public sealed record GetUserByEmailQuery(string Email) : IRequest<UserIdentity?>;
public sealed record GetAllUsersQuery : IRequest<UserIdentity[]>;
public sealed record TestUser(int Id, string Firstname, string Lastname);
public sealed record FindAllTestUsersQuery(string? QueryTerm, string? SortColumn, string? SortOrder, int PageNumber, int PageSize) : IRequest<TestUser[]>;