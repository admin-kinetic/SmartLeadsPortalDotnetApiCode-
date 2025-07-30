using Microsoft.Data.SqlClient;
using MySqlConnector;
using Polly;
using Polly.Retry;
using System.Data;
using System.Data.Common;

namespace SmartLeadsPortalDotNetApi.Database;

public class DbConnectionFactory : IDisposable
{
    private readonly string callsSqlConnectionString;
    private readonly object connectionLock = new object();
    private readonly AsyncRetryPolicy<DbConnection> retryPolicy;
    private readonly ILogger<DbConnectionFactory> logger;
    private SqlConnection? callsSqlConnection;
    private bool disposed = false;

    // Azure SQL specific error codes
    private static readonly int[] RetryableSqlErrors = new[]
    {
        10928, // Resource ID: %d. The %s limit for the database is %d and has been reached.
        10929, // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d.
        49918, // Cannot process request. Not enough resources to process request.
        49919, // Cannot process create or update request. Too many create or update operations in progress.
        49920  // The service is busy processing multiple requests for this subscription.
    };

    public DbConnectionFactory(IConfiguration configuration, ILogger<DbConnectionFactory> logger)
    {
        callsSqlConnectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")
            ?? configuration.GetConnectionString("SmartLeadsSQLServerDBConnectionString")
            ?? throw new InvalidOperationException("SmartleadsPortalDb connection string is missing.");

        this.logger = logger;

        // Configure retry policy with improved handling
        this.retryPolicy = Policy<DbConnection>
            .Handle<SqlException>(ex => 
                ex.IsTransient || 
                RetryableSqlErrors.Contains(ex.Number))
            .WaitAndRetryAsync(
                5, // Increased retry attempts
                retryAttempt =>
                {
                    // Exponential backoff with some randomization to prevent multiple clients from retrying simultaneously
                    var backoff = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000));
                    return backoff + jitter;
                },
                onRetry: (exception, timeSpan, retryCount, _) =>
                {
                    if (exception.Exception is SqlException sqlEx)
                    {
                        this.logger.LogWarning("Attempt {RetryCount} failed with SQL error {SqlError}. Waiting {DelaySeconds} seconds before next retry.",
                            retryCount,
                            sqlEx.Number,
                            timeSpan.TotalSeconds);
                    }
                    else
                    {
                        this.logger.LogWarning("Attempt {RetryCount} failed. Waiting {DelaySeconds} seconds before next retry.",
                            retryCount,
                            timeSpan.TotalSeconds);
                    }
                }
            );
    }

    public DbConnection GetSqlConnection()
    {
        return GetSqlConnectionAsync().GetAwaiter().GetResult();
    }

    public async Task<DbConnection> GetSqlConnectionAsync()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException(nameof(DbConnectionFactory), "Cannot access a disposed connection factory");
        }

        return await retryPolicy.ExecuteAsync(async () =>
        {
            lock (connectionLock)
            {
                if (this.callsSqlConnection == null || this.callsSqlConnection.State == ConnectionState.Closed || this.callsSqlConnection.State == ConnectionState.Broken)
                {
                    this.callsSqlConnection = new SqlConnection(this.callsSqlConnectionString);
                }
            }

            if (this.callsSqlConnection.State != ConnectionState.Open)
            {
                await this.callsSqlConnection.OpenAsync();
            }

            return this.callsSqlConnection;
        });
    }

    public void ValidateConnections()
    {
        try
        {
            using (var sqlConnection = GetSqlConnection())
            {
                this.logger.LogInformation("Successfully connected to SQL Server database.");
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Failed to connect to SQL Server database: {ex.Message}");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                if (this.callsSqlConnection != null)
                {
                    try
                    {
                        if (this.callsSqlConnection.State != ConnectionState.Closed)
                            this.callsSqlConnection.Close();
                    }
                    catch { /* Ignore errors during dispose */ }
                    finally
                    {
                        this.callsSqlConnection.Dispose();
                    }
                }
            }

            disposed = true;
        }
    }
}

