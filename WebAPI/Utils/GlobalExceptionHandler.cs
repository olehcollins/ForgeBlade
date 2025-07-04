using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace WebAPI.Utils;

[ExcludeFromCodeCoverage (Justification = "Middle Not Part of Testing")]
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception is ApplicationException
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;

        Log.Error(exception, exception.Message);

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An Error has occurred",
                Detail = exception.Message
            }
        });
    }
}