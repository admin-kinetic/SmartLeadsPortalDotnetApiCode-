
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

    public async Task InsertWebhook(string payload)
    {
        logger.LogInformation($"Inserting voipline webhook into database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var insert = @"INSERT INTO VoiplineWebhooks (Request, CreatedAt) VALUES (@payload, GETDATE());";
            await connection.ExecuteAsync(insert, new { payload });
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