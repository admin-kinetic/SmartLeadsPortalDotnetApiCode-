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
        try
        {
            using var connection = _dbConnectionFactory.GetSqlConnection();
            var upsert = """
                MERGE INTO SmartLeadsEmailStatistics AS target
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting open email.");
            throw;
        }
    }

    public async Task UpsertEmailLinkClickedCount(EmailLinkClickedPayload emaiLinkClickedPayload)
    {
        try
        {
            using var connection = _dbConnectionFactory.GetSqlConnection();
            var upsert = """
                MERGE INTO SmartLeadsEmailStatistics AS target
                USING ( 
                    VALUES (@leadEmail, @sequenceNumber)
                ) AS source (LeadEmail, SequenceNumber)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        ClickCount = target.ClickCount + 1, 
                        ClickTime = @timeClicked
                WHEN NOT MATCHED THEN
                    INSERT (Guid, LeadEmail, LeadName, SequenceNumber, EmailSubject, ClickCount, ClickTime)
                        VALUES (NewId(), @leadEmail, @leadName, @sequenceNumber, @emailSubject, 1, @timeClicked);
            """;

            await connection.ExecuteAsync(upsert,
                new
                {
                    leadEmail = emaiLinkClickedPayload.to_email,
                    leadName = emaiLinkClickedPayload.to_name,
                    sequenceNumber = emaiLinkClickedPayload.sequence_number,
                    emailSubject = emaiLinkClickedPayload.subject,
                    timeClicked = emaiLinkClickedPayload.time_clicked,
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting open email.");
            throw;
        }
    }

    internal async Task UpsertEmailSent(EmailSentPayload emailOpenPayload)
    {
        try
        {
            using var connection = _dbConnectionFactory.GetSqlConnection();
            var upsert = """
                MERGE INTO SmartLeadsEmailStatistics AS target
                USING ( 
                    VALUES (@leadEmail, @sequenceNumber)
                ) AS source (LeadEmail, SequenceNumber)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        LeadId = @leadId,
                        LeadName = @leadName,
                        EmailSubject = @emailSubject,
                        EmailMessage = @emailMessage
                WHEN NOT MATCHED THEN
                    INSERT (Guid, LeadId, LeadEmail, LeadName, SequenceNumber, EmailSubject, EmailMessage)
                        VALUES (NewId(), @leadId, @leadEmail, @leadName, @sequenceNumber, @emailSubject, @emailMessage);
            """;

            await connection.ExecuteAsync(upsert,
                new
                {
                    leadId = emailOpenPayload.sl_email_lead_id,
                    leadEmail = emailOpenPayload.to_email,
                    leadName = emailOpenPayload.to_name,
                    sequenceNumber = emailOpenPayload.sequence_number,
                    emailSubject = emailOpenPayload.subject,
                    emailMessage = emailOpenPayload.sent_message_body
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting open email.");
            throw;
        }
    }
}
