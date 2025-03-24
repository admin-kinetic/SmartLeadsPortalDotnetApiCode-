using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Database
{
    public class DbConnectionFactory: IDisposable
    {
        private readonly string _connectionString;
        private readonly string _mysqlconnectionString;
        private IDbConnection _connection;

        public DbConnectionFactory(IConfiguration configuration)
        {

            _mysqlconnectionString = configuration.GetConnectionString("MySQLDBConnectionString");
            _connectionString = configuration.GetConnectionString("SQLServerDBConnectionString");
        }

        public IDbConnection GetConnection()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }

            return _connection;
        }

        public IDbConnection GetConnectionMySQL()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = new MySqlConnection(_mysqlconnectionString);
                _connection.Open();
            }

            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
