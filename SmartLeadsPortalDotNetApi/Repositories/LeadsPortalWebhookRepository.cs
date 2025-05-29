using System;
using Dapper;
using Microsoft.Graph.Models;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class LeadsPortalWebhookRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public LeadsPortalWebhookRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task Insert(string payload, string type)
    {
        using (var connection = dbConnectionFactory.GetSqlConnection())
        {
            var insert = @"INSERT INTO LeadsPortalWebhooks (Request, CreatedAt, Type) VALUES (@payload, GETDATE(), @type);";
            await connection.ExecuteAsync(insert, new { payload, type});
        }
    }
}
