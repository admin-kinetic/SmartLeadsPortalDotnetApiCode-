using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Services
{
    public class SQLDBService : IDisposable
    {
        protected IDbConnection con;
        protected IDbConnection leadcon;
        protected IDbConnection mysqlcon;
        private static IConfiguration configuration;

        public IDbConnection getConnection()
        {
            return con;
        }
        public IDbConnection getLeadConnection()
        {
            return leadcon;
        }

        public IDbConnection getMysqlConnection()
        {
            return mysqlcon;
        }
        public SQLDBService()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            var connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")
               ?? configuration.GetConnectionString("SmartLeadsSQLServerDBConnectionString")
               ?? throw new InvalidOperationException("SmartleadsPortalDb connection string is missing.");
            con = new SqlConnection(connectionString);
            
            // leadcon = new SqlConnection(configuration.GetConnectionString("LeadsPortalSQLServerDBConnectionString"));
            // mysqlcon = new MySqlConnection(configuration.GetConnectionString("MySQLDBConnectionString"));
        }
        public void CheckIfOpen()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                if (leadcon.State == ConnectionState.Closed)
                {
                    leadcon.Open();
                }

                if (mysqlcon.State == ConnectionState.Closed)
                {
                    mysqlcon.Open();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (con == null)
                {
                    con.Open();
                }

                if (leadcon == null)
                {
                    leadcon.Open();
                }

                if (mysqlcon == null)
                {
                    mysqlcon.Open();
                }
            }

            //throw new NotImplementedException(); 


        }

        public void Dispose()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                if (leadcon.State == ConnectionState.Open)
                {
                    leadcon.Close();
                }

                if (mysqlcon.State == ConnectionState.Open)
                {
                    mysqlcon.Close();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                }

                if (leadcon != null)
                {
                    leadcon.Dispose();
                }

                if (mysqlcon != null)
                {
                    mysqlcon.Dispose();
                }
            }

            //throw new NotImplementedException(); 


        }
    }
}
