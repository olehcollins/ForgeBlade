using System.Linq.Expressions;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.QueryHandlers;

public class UserQueriesHandler(UserManager<UserIdentity> userManager, ApplicationDbContext dbContext) :
    IRequestHandler<GetUserByIdQuery, UserIdentity?>,
    IRequestHandler<GetUserByEmailQuery, UserIdentity?>,
    IRequestHandler<GetAllUsersQuery, UserIdentity[]>
{
    public async Task<UserIdentity?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken) => await userManager.FindByIdAsync(request.UserId);

    public async Task<UserIdentity?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken) => await userManager.FindByEmailAsync(request.Email);

    public async Task<UserIdentity[]> Handle(GetAllUsersQuery request, CancellationToken cancellationToken) => await userManager.Users.ToArrayAsync(cancellationToken: cancellationToken);
}