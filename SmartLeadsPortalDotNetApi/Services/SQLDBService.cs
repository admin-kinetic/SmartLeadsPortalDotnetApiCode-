using Microsoft.Data.SqlClient;
using MySqlConnector;
using System.Data;

namespace SmartLeadsPortalDotNetApi.Services
{
    public class SQLDBService : IDisposable
    {
        protected IDbConnection con;
        protected IDbConnection mysqlcon;
        private static IConfiguration configuration;

        public IDbConnection getConnection()
        {
            return con;
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
            con = new SqlConnection(configuration.GetConnectionString("SQLServerDBConnectionString"));
            mysqlcon = new MySqlConnection(configuration.GetConnectionString("MySQLDBConnectionString"));
        }
        public void CheckIfOpen()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
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

                if (mysqlcon != null)
                {
                    mysqlcon.Dispose();
                }
            }

            //throw new NotImplementedException(); 


        }
    }
}
