using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Database;
public class DbConnectionFactory : IDisposable
{
    private readonly string _sqlConnectionString;
    //private readonly string _leadsqlConnectionString;
    //private readonly string _mysqlConnectionString;
    private readonly ILogger<DbConnectionFactory> logger;
    private IDbConnection? _sqlConnection;
    //private IDbConnection? _leadsqlConnection;
    //private IDbConnection? _mySqlConnection;

    public DbConnectionFactory(IConfiguration configuration, ILogger<DbConnectionFactory> logger)
    {
        _sqlConnectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")
            ?? configuration.GetConnectionString("SmartLeadsSQLServerDBConnectionString")
            ?? throw new InvalidOperationException("SmartleadsPortalDb connection string is missing.");

        // _leadsqlConnectionString = configuration.GetConnectionString("LeadsPortalSQLServerDBConnectionString")
        //     ?? throw new ArgumentNullException(nameof(configuration), "Robotics Leads SQL Server connection string is missing.");

        // _mysqlConnectionString = configuration.GetConnectionString("MySQLDBConnectionString")
        //     ?? throw new ArgumentNullException(nameof(configuration), "MySQL connection string is missing.");
        this.logger = logger;
        this.logger.LogInformation($"SQL Connection String From Environment: {Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")}");
        this.logger.LogInformation($"SQL Connection String: {this._sqlConnectionString}");
    }

    public IDbConnection GetSqlConnection()
    {
        return new SqlConnection(_sqlConnectionString);
    }

    //public IDbConnection GetLeadSqlConnection()
    //{
    //    if (_leadsqlConnection == null || _leadsqlConnection.State == ConnectionState.Closed)
    //    {
    //        _leadsqlConnection = CreateConnection(_leadsqlConnectionString, () => new SqlConnection(_leadsqlConnectionString));
    //    }
    //    return _leadsqlConnection;
    //}

    //public IDbConnection GetMySqlConnection()
    //{
    //    if (_mySqlConnection == null || _mySqlConnection.State == ConnectionState.Closed)
    //    {
    //        _mySqlConnection = CreateConnection(_mysqlConnectionString, () => new MySqlConnection(_mysqlConnectionString));
    //    }

    //    return _mySqlConnection;
    //}

    private IDbConnection CreateConnection(string connectionString, Func<IDbConnection> connectionFactory)
    {
        var connection = connectionFactory();
        try
        {
            connection.Open();
        }
        catch
        {
            connection.Dispose();
            throw;
        }

        return connection;
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

        //try
        //{
        //    using (var leadSqlConnection = GetLeadSqlConnection())
        //    {
        //        this.logger.LogInformation("Successfully connected to Leads SQL Server database.");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    this.logger.LogError($"Failed to connect to Leads SQL Server database: {ex.Message}");
        //}

        // try
        // {
        //     using (var mySqlConnection = GetMySqlConnection())
        //     {
        //         this.logger.LogInformation("Successfully connected to MySQL database.");
        //     }
        // }
        // catch (Exception ex)
        // {
        //     this.logger.LogError($"Failed to connect to MySQL database: {ex.Message}");
        // }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sqlConnection?.Dispose();
            //_leadsqlConnection?.Dispose();
            //_mySqlConnection?.Dispose();
        }
    }
}

