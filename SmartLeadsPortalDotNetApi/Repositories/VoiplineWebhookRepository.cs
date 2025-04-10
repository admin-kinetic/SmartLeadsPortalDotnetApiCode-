
using Dapper;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class VoiplineWebhookRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly ILogger<VoiplineWebhookRepository> logger;

    public VoiplineWebhookRepository(DbConnectionFactory dbConnectionFactory, ILogger<VoiplineWebhookRepository> logger)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.logger = logger;
    }

    public async Task<List<string>> GetAllUniqueCallId(){
        using( var connection = dbConnectionFactory.GetSqlConnection()){
            var query = """
                Select UniqueCallId From VoiplineWebhooks
                Order By Id DESC;
            """;
            var result = await connection.QueryAsync<string>(query);
            return result.Distinct().ToList();
        }
    }

    public async Task InsertWebhook(string webhookType, string payload)
    {
        logger.LogInformation($"Inserting voipline webhook into database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var insert = @"INSERT INTO VoiplineWebhooks (Type, Request, CreatedAt) VALUES (@webhookType, @payload, GETDATE());";
            await connection.ExecuteAsync(insert, new { payload, webhookType });
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                $"Inserted voipline webhook into database in {stopwatch.ElapsedMilliseconds} ms"
            );
        }
    }
}