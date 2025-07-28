using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartLeadCampaignsRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public SmartLeadCampaignsRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<List<SmartLeadCampaign>> GetCampaignsAsync()
    {
        using var connection = await dbConnectionFactory.GetSqlConnectionAsync();
        var query = """
            Select * From SmartLeadCampaigns
            Where [Name] <> '' OR [Name] IS NULL
            Order By Name Asc
        """;
        var queryResult = await connection.QueryAsync<SmartLeadCampaign>(query);
        return queryResult.ToList();
    }
}
