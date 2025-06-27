using System.Transactions;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;
using SmartLeadsPortalDotNetApi.Services.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartLeadsEmailStatisticsRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<SmartLeadsEmailStatisticsRepository> _logger;

    public SmartLeadsEmailStatisticsRepository(DbConnectionFactory dbConnectionFactory, ILogger<SmartLeadsEmailStatisticsRepository> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task UpsertEmailOpenCount(EmailOpenPayload emailOpenPayload)
    {

        _logger.LogInformation("Start UpsertEmailOpenCount");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        using var connection = _dbConnectionFactory.GetSqlConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }


        var upsert = """
                MERGE INTO SmartLeadsEmailStatistics WITH (ROWLOCK) AS target
                USING ( 
                    VALUES (@leadEmail, @sequenceNumber)
                ) AS source (LeadEmail, SequenceNumber)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        OpenCount = target.OpenCount + 1, 
                        OpenTime = @openTime
                WHEN NOT MATCHED THEN
                    INSERT (Guid, LeadEmail, LeadName, SequenceNumber, EmailSubject, OpenCount, OpenTime)
                        VALUES (NewId(), @leadEmail, @leadName, @sequenceNumber, @emailSubject, 1, @openTime);
            """;

        await connection.ExecuteAsync(upsert,
            new
            {
                leadEmail = emailOpenPayload.to_email,
                leadName = emailOpenPayload.to_name,
                sequenceNumber = emailOpenPayload.sequence_number,
                emailSubject = emailOpenPayload.subject,
                openTime = emailOpenPayload.time_opened

            });
        _logger.LogInformation("UpsertEmailOpenCount took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

    }

    public async Task UpsertEmailLinkClickedCount(EmailLinkClickedPayload emaiLinkClickedPayload)
    {
        _logger.LogInformation("Start UpsertEmailLinkClickedCount");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var connection = await _dbConnectionFactory.GetSqlConnectionAsync();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var upsert = """
                MERGE INTO SmartLeadsEmailStatistics  WITH (ROWLOCK) AS target
                USING ( 
                    VALUES (@leadEmail, @sequenceNumber)
                ) AS source (LeadEmail, SequenceNumber)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        ClickCount = target.ClickCount + 1, 
                        ClickTime = @timeClicked
                WHEN NOT MATCHED THEN
                    INSERT (Guid, LeadId, LeadEmail, LeadName, SequenceNumber, EmailSubject, ClickCount, ClickTime)
                        VALUES (NewId(), @LeadId, @leadEmail, @leadName, @sequenceNumber, @emailSubject, 1, @timeClicked);
            """;

        await connection.ExecuteAsync(upsert,
            new
            {
                leadEmail = emaiLinkClickedPayload.to_email,
                leadId = emaiLinkClickedPayload.sl_email_lead_id,
                leadName = emaiLinkClickedPayload.to_name,
                sequenceNumber = emaiLinkClickedPayload.sequence_number,
                emailSubject = emaiLinkClickedPayload.subject,
                timeClicked = emaiLinkClickedPayload.time_clicked,
            });

        _logger.LogInformation("UpsertEmailLinkClickedCount took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);


    }

    internal async Task UpsertEmailSent(EmailSentPayload emailOpenPayload)
    {
        _logger.LogInformation("Start UpsertEmailSent");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var connection = await _dbConnectionFactory.GetSqlConnectionAsync();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var transaction = await connection.BeginTransactionAsync();

        try 
        {
            var parameters = new
            {
                leadId = emailOpenPayload.sl_email_lead_id,
                leadEmail = emailOpenPayload.to_email,
                leadName = emailOpenPayload.to_name,
                sequenceNumber = emailOpenPayload.sequence_number,
                emailSubject = emailOpenPayload.subject,
                emailMessage = emailOpenPayload.sent_message_body,
                sentTime = emailOpenPayload.time_sent
            };

            // First check if the record exists
            var existingRecord = await connection.QueryFirstOrDefaultAsync<int>("""
                SELECT 1 FROM SmartLeadsEmailStatistics 
                WHERE LeadEmail = @leadEmail AND SequenceNumber = @sequenceNumber
            """, parameters, transaction);

            if (existingRecord != 0)
            {
                // Update existing record
                await connection.ExecuteAsync("""
                    UPDATE SmartLeadsEmailStatistics 
                    SET LeadId = @leadId,
                        LeadName = @leadName,
                        EmailSubject = @emailSubject,
                        EmailMessage = @emailMessage,
                        SentTime = @sentTime
                    WHERE LeadEmail = @leadEmail 
                    AND SequenceNumber = @sequenceNumber
                """, parameters, transaction);
            }
            else
            {
                // Insert new record
                await connection.ExecuteAsync("""
                    INSERT INTO SmartLeadsEmailStatistics (
                        Guid, LeadId, LeadEmail, LeadName, 
                        SequenceNumber, EmailSubject, EmailMessage, SentTime
                    ) VALUES (
                        NewId(), @leadId, @leadEmail, @leadName, 
                        @sequenceNumber, @emailSubject, @emailMessage, @sentTime
                    )
                """, parameters, transaction);
            }

            await transaction.CommitAsync();
            _logger.LogInformation("UpsertEmailSent took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error in UpsertEmailSent for {Email}", emailOpenPayload.to_email);
            throw;
        }
    }

    internal async Task UpdateEmailReply(EmailReplyPayload payloadObject)
    {
        _logger.LogInformation($"Start Update email reply for {payloadObject.to_email} in statistics");
        await using var connection = await this._dbConnectionFactory.GetSqlConnectionAsync();
        var update = """
            UPDATE SmartLeadsEmailStatistics
            SET ReplyTime = @replyTime
            WHERE LeadEmail = @leadEmail AND SequenceNumber = @sequenceNumber
        """;
        var updateParam = new
        {
            leadEmail = payloadObject.to_email,
            sequenceNumber = payloadObject.sequence_number,
            replyTime = payloadObject.event_timestamp
        };
        await connection.ExecuteAsync(update, updateParam);
        _logger.LogInformation($"Succesfuly updated email reply for {payloadObject.to_email} in statistics");
    }
}
