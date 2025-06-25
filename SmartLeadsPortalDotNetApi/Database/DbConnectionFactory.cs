using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace SmartLeadsPortalDotNetApi.Database;

public class DbConnectionFactory : IDisposable
{
    private readonly string callsSqlConnectionString;
    private readonly object connectionLock = new object();

    private readonly ILogger<DbConnectionFactory> logger;
    private SqlConnection callsSqlConnection;
    private bool disposed = false;
    public DbConnectionFactory(IConfiguration configuration, ILogger<DbConnectionFactory> logger)
    {
        callsSqlConnectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")
            ?? configuration.GetConnectionString("SmartLeadsSQLServerDBConnectionString")
            ?? throw new InvalidOperationException("SmartleadsPortalDb connection string is missing.");

        this.logger = logger;
        this.logger.LogInformation($"SQL Connection String From Environment: {Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")}");
        this.logger.LogInformation($"SQL Connection String: {this.callsSqlConnectionString}");
    }

    public DbConnection GetSqlConnection()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException(nameof(DbConnectionFactory), "Cannot access a disposed connection factory");
        }

        lock (connectionLock)
        {
            if (this.callsSqlConnection == null || this.callsSqlConnection.State == ConnectionState.Closed || this.callsSqlConnection.State == ConnectionState.Broken)
            {
                this.callsSqlConnection = new SqlConnection(this.callsSqlConnectionString);
            }
        }

        if (this.callsSqlConnection.State != ConnectionState.Open)
        {
            this.callsSqlConnection.Open();
        }

        return this.callsSqlConnection;
    }

    

    public async Task<SqlConnection> GetSqlConnectionAsync()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException(nameof(DbConnectionFactory), "Cannot access a disposed connection factory");
        }

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

