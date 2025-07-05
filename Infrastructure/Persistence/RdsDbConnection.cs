using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Amazon.Lambda.Core;
using Amazon.RDS.Util;
using Microsoft.Extensions.Configuration;
using Npgsql;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Infrastructure.Persistence;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public static class RdsDbConnection
{
    public static string GetConnectionString(IConfiguration configuration)
    {
         string rdsEndpoint = configuration["AWS:RDS_ENDPOINT"] ?? throw new InvalidOperationException("missing RDS_ENDPOINT value");
         string rdsPort = configuration["AWS:RDS_PORT"] ?? throw new InvalidOperationException("missing RDS_PORT value");
         string rdsUsername = configuration["AWS:RDS_USERNAME"] ?? throw new InvalidOperationException("missing RDS_USERNAME value");
         string dbName = configuration["AWS:DB_NAME"] ?? throw new InvalidOperationException("missing DB_NAME value");

        // Get authentication token
        var authToken = RDSAuthTokenGenerator.GenerateAuthToken(
            rdsEndpoint,
            Convert.ToInt32(rdsPort, CultureInfo.InvariantCulture),
            rdsUsername
        );

        // Build the Connection String with the Token
        // return new NpgsqlConnectionStringBuilder
        // {
        //     Host = rdsEndpoint,
        //     Port = Convert.ToInt32(rdsPort, CultureInfo.InvariantCulture),
        //     Username = rdsUsername,
        //     Password = authToken,
        //     Database = dbName,
        //     SslMode = SslMode.Require,
        // }.ToString();
        return configuration["ConnectionStrings:PostgresConnection"] ??  throw new InvalidOperationException("missing connection string");
    }
    public static async Task<bool> ConnectToDbAsync(IConfiguration configuration)
    {
        await using var connection = new NpgsqlConnection(GetConnectionString(configuration));
        await connection.OpenAsync();

        Console.WriteLine("✅ Connected to AWS RDS database!\n");
        return true;
    }
}