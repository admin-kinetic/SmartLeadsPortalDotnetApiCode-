using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartleadAccountRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;

    public SmartleadAccountRepository(DbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<SmartleadAccount?> GetAccountBySecretKey(string? secretKey)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        var query = """
            Select * From SmartleadsAccounts
            Where ApiKey LIKE @SecretKey
        """;
        var queryResult = await connection.QueryFirstOrDefaultAsync<SmartleadAccount>(query, new { SecretKey = $"{secretKey}%" });
        return queryResult;
    }

    public async Task InsertAccountCampaign(int? smartleadsAccountId, int? campaignId)
    {
        using var connection = this.dbConnectionFactory.GetSqlConnection();
        var query = """
            INSERT INTO SmartleadsAccountCampaigns (SmartleadsAccountId, CampaignId)
                VALUES (@smartleadsAccountId, @campaignId)
        """;
        await connection.ExecuteAsync(query, new { smartleadsAccountId, campaignId});
    }
}
