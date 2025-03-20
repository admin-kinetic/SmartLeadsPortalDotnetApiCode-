using System.Data;
using System.Data.SqlClient;

namespace SmartLeadsPortalDotNetApi.Services
{
    public class SQLDBService : IDisposable
    {
        protected IDbConnection con;
        private static IConfiguration configuration;

        public IDbConnection getConnection()
        {
            return con;
        }
        public SQLDBService()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            con = new SqlConnection(configuration.GetConnectionString("AzureSQLDBConnectionString"));
        }
        public void CheckIfOpen()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
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
            }

            //throw new NotImplementedException(); 


        }
    }
}
