using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Amazon.Lambda.Core;
using Amazon.RDS.Util;
using Npgsql;
using Serilog;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WebAPI.Utils;

[ExcludeFromCodeCoverage(Justification = "Not part of code testing")]
public sealed class RdsDbConnection(IConfiguration configuration)
{
    private readonly string _rdsEndpoint = configuration["AWS:RDS_ENDPOINT"] ?? throw new InvalidOperationException("missing RDS_ENDPOINT value");
    private readonly string _rdsPort = configuration["AWS:RDS_PORT"] ?? throw new InvalidOperationException("missing RDS_PORT value");
    private readonly string _rdsUsername = configuration["AWS:RDS_USERNAME"] ?? throw new InvalidOperationException("missing RDS_USERNAME value");
    private readonly string _dbName = configuration["AWS:DB_NAME"] ?? throw new InvalidOperationException("missing DB_NAME value");

    public string GetConnectionString()
    {
        // Get authentication token
        var authToken = RDSAuthTokenGenerator.GenerateAuthToken(
            _rdsEndpoint,
            Convert.ToInt32(_rdsPort, CultureInfo.InvariantCulture),
            _rdsUsername
        );

        // Build the Connection String with the Token
        return new NpgsqlConnectionStringBuilder
        {
            Host = _rdsEndpoint,
            Port = Convert.ToInt32(_rdsPort, CultureInfo.InvariantCulture),
            Username = _rdsUsername,
            Password = authToken,
            Database = _dbName,
            SslMode = SslMode.Require,
        }.ToString();
    }
    public async Task ConnectToDbAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(GetConnectionString());
            await connection.OpenAsync();

            Console.WriteLine("✅ Connected to AWS RDS database!\n");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"❌ Connection to AWS RDS database failed");
        }
    }
}