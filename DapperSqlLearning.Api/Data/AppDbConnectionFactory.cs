using System.Data.Common;
using Microsoft.Data.Sqlite;
using Npgsql;

namespace DapperSqlLearning.Api.Data;

public sealed class AppDbConnectionFactory(IConfiguration configuration) : IAppDbConnectionFactory
{
    public string Provider { get; } = (configuration["Database:Provider"] ?? "postgres").Trim().ToLowerInvariant();

    public DbConnection CreateConnection()
    {
        return Provider switch
        {
            "postgres" or "postgresql" or "npgsql" => new NpgsqlConnection(GetPostgresConnectionString()),
            "sqlite" => new SqliteConnection(GetSqliteConnectionString()),
            _ => throw new InvalidOperationException($"Unsupported database provider '{Provider}'. Supported: postgres, sqlite.")
        };
    }

    private string GetPostgresConnectionString()
    {
        return configuration.GetConnectionString("Postgres")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing PostgreSQL connection string. Configure ConnectionStrings:Postgres or ConnectionStrings:DefaultConnection.");
    }

    private string GetSqliteConnectionString()
    {
        var rawConnectionString = configuration.GetConnectionString("Sqlite")
            ?? "Data Source=dapper_learning.db";

        var builder = new SqliteConnectionStringBuilder(rawConnectionString);
        var dataSource = builder.DataSource;
        var runtimeBasePath = AppContext.BaseDirectory;

        if (string.IsNullOrWhiteSpace(dataSource))
        {
            builder.DataSource = Path.Combine(runtimeBasePath, "dapper_learning.db");
        }
        else if (!Path.IsPathRooted(dataSource) && dataSource != ":memory:")
        {
            builder.DataSource = Path.GetFullPath(Path.Combine(runtimeBasePath, dataSource));
        }

        if (builder.DataSource != ":memory:")
        {
            var directory = Path.GetDirectoryName(builder.DataSource);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        return builder.ConnectionString;
    }
}
