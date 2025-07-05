using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Infrastructure;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}