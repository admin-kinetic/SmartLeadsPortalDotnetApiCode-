using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ExportedLeadsFromLeadsPortal;

public class DbConnectionFactory
{
    private readonly string conectionString;
    public DbConnectionFactory(IConfiguration configuration)
    {
        this.conectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")
           ?? configuration?.GetConnectionString("SmartLeadsPortalDB")
           ?? throw new InvalidOperationException("SmartleadsPortalDb connection string is missing.");
        Console.WriteLine($"SQL Connection String From Environment: {Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SMARTLEADS_PORTAL_DB")}");
    }   

    public DbConnection CreateConnection()
    {
        return new SqlConnection(this.conectionString);
    }
}

