using MySqlConnector;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Database
{
    public class DbConnectionFactory: IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;

        public DbConnectionFactory(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("MySQLDBConnectionString");
        }

        public IDbConnection GetConnection()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                _connection = new MySqlConnection(_connectionString);
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
