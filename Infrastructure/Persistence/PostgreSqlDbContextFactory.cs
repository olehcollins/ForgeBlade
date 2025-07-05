using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Persistence
{
    public class PostgreSqlDbContextFactory
        : IDesignTimeDbContextFactory<PostgreSqlDbContext>
    {
        public PostgreSqlDbContext CreateDbContext(string[] args)
        {
            var webApiPath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(), "..", "WebApi"));
            var configuration = new ConfigurationBuilder()
                .SetBasePath(webApiPath)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<PostgreSqlDbContext>();
            var conn = RdsDbConnection.GetConnectionString(configuration)
                       ?? throw new InvalidOperationException("Connection string not found.");

            builder.UseNpgsql(conn);

            return new PostgreSqlDbContext(builder.Options);
        }
    }
}