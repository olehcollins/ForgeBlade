using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;
using Infrastructure.Persistence;

namespace WebAPI.Utils;

[ExcludeFromCodeCoverage(Justification = "Middle Not Part of Testing")]
public sealed class CustomHealthChecks(IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        var healthCheckData = new Dictionary<string, object>();
        var errors = new List<string>();

        var isDatabaseHealthy = await CheckDatabaseConnectionAsync();
        healthCheckData["Database"] = isDatabaseHealthy ? "Healthy" : "Failed";

        if (!isDatabaseHealthy)
        {
            errors.Add("Database connection failed.");
        }

        return errors.Count switch
        {
            0 => HealthCheckResult.Healthy(
                "All services are healthy.",
                healthCheckData
            ),
            1 => HealthCheckResult.Degraded(
                $"Some services are slow or degraded: {string.Join(", ", errors)}",
                data: healthCheckData
            ),
            _ => HealthCheckResult.Unhealthy(
                $"Some services are slow or degraded: {string.Join(", ", errors)}",
                data: healthCheckData
            )
        };
    }

    private async Task<bool> CheckDatabaseConnectionAsync() => await RdsDbConnection.ConnectToDbAsync(configuration);

}