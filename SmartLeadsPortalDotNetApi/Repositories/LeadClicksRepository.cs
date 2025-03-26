using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class LeadClicksRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly ILogger<LeadClicksRepository> logger;

    public LeadClicksRepository(DbConnectionFactory dbConnectionFactory, ILogger<LeadClicksRepository> logger)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.logger = logger;
    }

    public async Task<List<dynamic>> GetLeadClicks()
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var select = "SELECT * FROM LeadClicks";
            var result = await connection.QueryAsync<dynamic>(select);
            return result.ToList();
        }
        catch(Exception ex)
        {
            this.logger.LogError(ex, "Error getting click count.");
            throw;
        }
    }

    public async Task<int> GetClickCountById(int leadId)
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var select = "SELECT ClickCount FROM LeadClicks WHERE LeadId = @leadId;";
            return await connection.QueryFirstOrDefaultAsync<int>(select, new { leadId });
        }
        catch(Exception ex)
        {
            this.logger.LogError(ex, "Error getting click count.");
            throw;
        }
    }

    public async Task UpsertById(int leadId)
    {
        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var insert = """
                MERGE INTO LeadClicks AS target
                USING (VALUES (@leadId)) AS source (LeadId)
                ON target.LeadId = source.LeadId
                WHEN MATCHED THEN
                    UPDATE SET 
                        ClickCount = target.ClickCount + 1, 
                        LatestClickDateTime = GETDATE()
                WHEN NOT MATCHED THEN
                    INSERT (LeadId, ClickCount, LatestClickDateTime)
                    VALUES (source.LeadId, 1, GETDATE());
            """;

            await connection.ExecuteAsync(insert, new { leadId });
        }
        catch(Exception ex)
        {
            this.logger.LogError(ex, "Error upserting lead click.");
            throw;
        }
    }
}
