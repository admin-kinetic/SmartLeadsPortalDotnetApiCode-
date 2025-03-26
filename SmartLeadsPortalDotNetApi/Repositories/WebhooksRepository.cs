using System;
using Dapper;
using Microsoft.Extensions.Logging;
using SmartLeadsPortalDotNetApi.Database;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class WebhooksRepository
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly ILogger<WebhooksRepository> logger;

    public WebhooksRepository(DbConnectionFactory dbConnectionFactory, ILogger<WebhooksRepository> logger)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.logger = logger;
    }

    public async Task InsertWebhook(string payload)
    {
        logger.LogInformation($"Inserting webhook into database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var insert = @"INSERT INTO Webhooks (Request, CreatedAt) VALUES (@payload, GETDATE());";
            await connection.ExecuteAsync(insert, new { payload });
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                $"Inserted webhook into database in {stopwatch.ElapsedMilliseconds} ms"
            );
        }
    }
}
