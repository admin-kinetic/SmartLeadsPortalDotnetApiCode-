using System;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartLeadsAllLeadsRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public SmartLeadsAllLeadsRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task UpsertLeadFromEmailSent(EmailSentPayload payload)
    {
        using var connection = _dbConnectionFactory.GetSqlConnection();

        var lead = new
        {
            LeadId = payload.sl_email_lead_id,
            Email = payload.to_email,
            CampaignId = payload.campaign_id,
            TimeSent = payload.time_sent,
        };

        var upsert = """
            MERGE INTO SmartLeadAllLeads AS target
            USING (SELECT 
                        @LeadId AS LeadId,
                        @Email AS Email,
                        @CampaignId AS CampaignId) AS source
            ON (target.LeadId = source.LeadId)
            WHEN MATCHED THEN
                UPDATE SET
                    Email = source.Email,
                    CampaignId = source.CampaignId
            WHEN NOT MATCHED THEN
                INSERT (LeadId, Email, CampaignId, CreatedAt, LeadStatus)
                VALUES (source.LeadId, source.Email, source.CampaignId, @TimeSent, 'INPROGRESS');
        """;

        await connection.ExecuteAsync(upsert, lead);
    }
}
