using Common.Database;
using Dapper;
using ReprocessEmailSentWebhook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReprocessEmailSentWebhook;

public class ReprocessEmailSentWebhookService
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public ReprocessEmailSentWebhookService(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    public async Task Run()
    {
        var emailSentWebhooks = await GetEmailSentWebhooksForReprocessing();

        if (!emailSentWebhooks.Any())
        {
            Console.WriteLine("No EMAIL_SENT webhooks to reprocess.");
        }

        Console.WriteLine($"Reprocessing {emailSentWebhooks.Count} EMAIL_SENT webhooks...");

        foreach (var emailSentWebhook in emailSentWebhooks)
        {
            await this.HandleEmailSent(emailSentWebhook);
        }   
    }

    private async Task<List<string>> GetEmailSentWebhooksForReprocessing()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        var query = """
                SELECT 
                    wh.Request
                FROM 
                    Webhooks wh
                INNER JOIN SmartLeadsEmailStatistics sles ON 
                    sles.LeadEmail = JSON_VALUE(wh.Request, '$.to_email') AND
                    sles.SentTime IS NULL
                INNER JOIN SmartLeadAllLeads slal ON 
                    slal.LeadId = sles.LeadId
                INNER JOIN SmartLeadCampaigns slc ON 
                    slc.Id = slal.CampaignId
                WHERE 
                    wh.EventType = 'EMAIL_SENT'
                ORDER BY 
                    slal.CreatedAt ASC;
            """;
        var result = await connection.QueryAsync<string>(query);
        return result.ToList();
    }

    private async Task HandleEmailSent(string payload)
    {

        var emailSentPayload = JsonSerializer.Deserialize<EmailSentPayload>(payload);

        var email = emailSentPayload.to_email;
        if (email == null || string.IsNullOrWhiteSpace(email.ToString()))
        {
            throw new ArgumentNullException("to_email", "Email is required.");
        }

        var sequenceNumber = emailSentPayload.sequence_number;
        await this.UpsertEmailSent(emailSentPayload);
    }

    internal async Task UpsertEmailSent(EmailSentPayload emailOpenPayload)
    {
        Console.WriteLine($"UpsertEmailSent for {emailOpenPayload.to_email}");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        using var connection = _dbConnectionFactory.CreateConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var upsert = """
                MERGE INTO SmartLeadsEmailStatistics WITH (ROWLOCK) AS target
                USING ( 
                    VALUES (@leadEmail, @sequenceNumber)
                ) AS source (LeadEmail, SequenceNumber)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        LeadId = @leadId,
                        LeadName = @leadName,
                        EmailSubject = @emailSubject,
                        EmailMessage = @emailMessage,
                        SentTime = @sentTime
                WHEN NOT MATCHED THEN
                    INSERT (Guid, LeadId, LeadEmail, LeadName, SequenceNumber, EmailSubject, EmailMessage, SentTime)
                        VALUES (NewId(), @leadId, @leadEmail, @leadName, @sequenceNumber, @emailSubject, @emailMessage, @sentTime);
            """;

            await connection.ExecuteAsync(upsert,
                new
                {
                    leadId = emailOpenPayload.sl_email_lead_id,
                    leadEmail = emailOpenPayload.to_email,
                    leadName = emailOpenPayload.to_name,
                    sequenceNumber = emailOpenPayload.sequence_number,
                    emailSubject = emailOpenPayload.subject,
                    emailMessage = emailOpenPayload.sent_message_body,
                    sentTime = emailOpenPayload.time_sent
                },
                transaction);
            await transaction.CommitAsync();
            Console.WriteLine($"UpsertEmailSent took {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error on UpsertEmailSent", ex);
            throw;
        }
    }
}
