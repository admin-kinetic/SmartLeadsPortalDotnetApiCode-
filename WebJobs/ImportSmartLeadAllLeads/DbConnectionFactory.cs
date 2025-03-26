using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ExportedLeadsFromLeadsPortal;

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

