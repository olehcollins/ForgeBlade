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
    IRequestHandler<GetAllUsersQuery, UserIdentity[]>,
    IRequestHandler<FindAllTestUsersQuery, TestUser[]>
{
    public async Task<UserIdentity?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken) => await userManager.FindByIdAsync(request.UserId);

    public async Task<UserIdentity?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken) => await userManager.FindByEmailAsync(request.Email);

    public async Task<UserIdentity[]> Handle(GetAllUsersQuery request, CancellationToken cancellationToken) => await userManager.Users.ToArrayAsync(cancellationToken: cancellationToken);

    public async Task<TestUser[]> Handle(FindAllTestUsersQuery request,
        CancellationToken cancellationToken)
    {
        var dataQuery = dbContext.TestUsers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.QueryTerm))
        {
            dataQuery = dataQuery.Where(u => EF.Functions.Like(u.Firstname, $"%{request.QueryTerm}%"));
        }

        dataQuery = request.SortOrder is "desc"
            ? dataQuery.OrderByDescending(GetKeySelector(request.SortColumn))
            : dataQuery.OrderBy(GetKeySelector(request.SortColumn));

        var totalCount = await dataQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
        if (totalPages < 1)
        {
            totalPages = 1;
        }

        int pageNumber = request.PageNumber > totalPages ? totalPages : request.PageNumber;

        dataQuery = dataQuery
            .Skip((pageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        return await dataQuery.ToArrayAsync(cancellationToken);
    }

    private static Expression<Func<TestUser, object>> GetKeySelector(string? sortColumn) => sortColumn switch
    {
        "firstname" => user => user.Firstname,
        "lastname" => user => user.Lastname,
        _ => u => u.Id
    };
}