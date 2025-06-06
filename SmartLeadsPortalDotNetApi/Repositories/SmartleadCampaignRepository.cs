using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartleadCampaignRepository
{
    private DbConnectionFactory dbConnectionFactory;

    public SmartleadCampaignRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<string?> GetCampaignBdr(int campaignId)
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            var query = """
                    Select Bdr From SmartLeadCampaigns
                    Where Id = @CampaignId
                """;
            var result = await connection.QueryFirstOrDefaultAsync<string>(query, new { campaignId });
            return result;
        }
    }

    public async Task<List<string>?> GetBdrs()
    {
        using (var connection = this.dbConnectionFactory.GetSqlConnection())
        {
            var query = """
                    Select Distinct Bdr 
                    From SmartleadCampaigns
                    Where (Bdr IS NOT NULL AND BDR != '')
                """;
            var queryResult = await connection.QueryAsync<string>(query);
            return queryResult.ToList();
        }
    }
}
