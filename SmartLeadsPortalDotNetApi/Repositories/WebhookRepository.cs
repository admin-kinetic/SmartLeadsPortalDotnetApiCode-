using System;
using Dapper;
using Microsoft.Extensions.Logging;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class WebhookRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly ILogger<WebhookRepository> logger;

    public WebhookRepository(DbConnectionFactory dbConnectionFactory, ILogger<WebhookRepository> logger)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.logger = logger;
    }

    public async Task InsertWebhook(string webhookType, string payload)
    {
        logger.LogInformation($"Inserting webhook \"{webhookType}\" into database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetConnection();
            var insert = @"INSERT INTO Webhooks (Request, CreatedAt) VALUES (@payload, NOW());";
            await connection.ExecuteAsync(insert, new { payload });
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                $"Inserted webhook \"{webhookType}\" into database in {stopwatch.ElapsedMilliseconds} ms"
            );
        }
    }
}
