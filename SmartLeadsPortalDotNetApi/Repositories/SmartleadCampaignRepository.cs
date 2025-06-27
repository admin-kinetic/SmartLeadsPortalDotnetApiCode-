using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Helper;
using SmartLeadsPortalDotNetApi.Services.Model;

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

    public async Task<SmartleadAccount> GetAccountByCampaignId(int? campaignId)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        var query = """
            Select sla.* From SmartLeadCampaigns slc
            Inner Join SmartleadsAccountCampaigns slac ON slac.CampaignId = slc.Id
            Inner Join SmartleadsAccounts sla On sla.Id = slac.SmartleadsAccountId
            Where slc.Id = @campaignId
        """;

        var queryResult = await connection.QueryFirstOrDefaultAsync<SmartleadAccount>(query, new { campaignId });
        return queryResult;
    }

    public async Task InsertCampaign(SmartLeadsCampaign? campaignDetails)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        var query = """
            INSERT INTO SmartLeadCampaigns (Id, Name, Status, Bdr)
            VALUES (@Id, @Name, @Status, @Bdr)
        """;
        await connection.ExecuteAsync(query, new
        {
            Id = campaignDetails?.id,
            Name = campaignDetails?.name,
            Status = campaignDetails?.status,
            bdr = BdrHelper.CampaignToBdrMapping.FirstOrDefault(name => campaignDetails.name.Contains(name, StringComparison.OrdinalIgnoreCase))
        });
    }
}
