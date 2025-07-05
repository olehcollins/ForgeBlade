using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;

namespace WebAPI.Utils;

[ExcludeFromCodeCoverage(Justification = "Code not part of test coverage")]
public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sandbox API",
                Version = "v1",
                Description = "API project for learning and experimenting"
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app,
        IHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sandbox API");
            c.RoutePrefix = string.Empty; // Make Swagger UI the root page
        });
        return app;
    }
}