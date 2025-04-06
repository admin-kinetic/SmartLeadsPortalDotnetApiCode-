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

    public async Task<dynamic> GetLeadClick()
    {
        logger.LogInformation($"Inserting webhook into database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var query = """
                Select JSON_VALUE(Request, '$.sl_email_lead_id') AS SmartleadId, JSON_VALUE(Request, '$.to_email') AS Email, Count(*) ClickCount From Webhooks
                Where JSON_VALUE(Request, '$.time_clicked') IS NOT NULL
                Group By JSON_VALUE(Request, '$.to_email'), JSON_VALUE(Request, '$.sl_email_lead_id');
                """;
            var result = await connection.QueryAsync<dynamic>(query);
            return result;
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
