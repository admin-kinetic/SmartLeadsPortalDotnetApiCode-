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

    public async Task InsertWebhook(string eventType, string payload)
    {
        logger.LogInformation($"Inserting webhook into database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await using var connection = await dbConnectionFactory.GetSqlConnectionAsync();
            var insert = @"INSERT INTO Webhooks (EventType, Request, CreatedAt) VALUES (@eventType, @payload, GETDATE());";
            await connection.ExecuteAsync(insert, new { eventType, payload });
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

    public async Task<List<string>> GetEmailReplyWebhooks()
    {
        logger.LogInformation($"Getting email reply webhook data from database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var query = """
                Select Request From Webhooks
                Where JSON_VALUE(Request,'$.event_type') = 'EMAIL_REPLY'
                ORder By Id Asc; 
                """;
            var result = await connection.QueryAsync<string>(query);
            return result.ToList();
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                $"Getting email reply webhook data from database in {stopwatch.ElapsedMilliseconds} ms"
            );
        }
    }

    public async Task<List<string>> GetLeadCategoryUpdated()
    {
        logger.LogInformation($"Getting email reply webhook data from database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var query = """
                Select Request From Webhooks
                Where JSON_VALUE(Request,'$.event_type') = 'LEAD_CATEGORY_UPDATED'
                ORDER By Id Asc; 
                """;
            var result = await connection.QueryAsync<string>(query);
            return result.ToList();
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                $"Getting email reply webhook data from database in {stopwatch.ElapsedMilliseconds} ms"
            );
        }
    }

    public async Task<List<string>> GetLeadCategoryUpdatedByWebhookId(int webhookId)
    {
        logger.LogInformation($"Getting email reply webhook data from database");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            using var connection = dbConnectionFactory.GetSqlConnection();
            var query = """
                Select Request From Webhooks
                Where JSON_VALUE(Request,'$.event_type') = 'LEAD_CATEGORY_UPDATED' AND Id = @webhookId
                ORDER By Id Asc; 
                """;
            var result = await connection.QueryAsync<string>(query, new { webhookId });
            return result.ToList();
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                $"Getting email reply webhook data from database in {stopwatch.ElapsedMilliseconds} ms"
            );
        }
    }
}
