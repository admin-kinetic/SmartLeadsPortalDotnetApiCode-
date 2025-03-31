using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ImportSmartLeadStatistics;

public class DbConnectionFactory
{
    private readonly string conectionString;
    public DbConnectionFactory(IConfiguration configuration)
    {
        this.conectionString = configuration?.GetConnectionString("SmartLeadsPortalDB");
    }   

    public DbConnection CreateConnection()
    {
        return new SqlConnection(this.conectionString);
    }
}
