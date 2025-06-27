using System.Diagnostics.CodeAnalysis;

namespace WebAPI.Utils;

[ExcludeFromCodeCoverage (Justification = "Middle Not Part of Testing")]
public sealed class CustomReqAndResMiddleWare : IMiddleware
{
   public async Task InvokeAsync(HttpContext context,  RequestDelegate next)
   {
      // nosniff header provides security for the browser.
      context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

      await next(context);
   }
}

public static class CustomReqAndResMiddleWareExtension
{
   public static IApplicationBuilder UseCustomReqAndResMiddleWare(this IApplicationBuilder app) =>
      app.UseMiddleware<CustomReqAndResMiddleWare>();
}