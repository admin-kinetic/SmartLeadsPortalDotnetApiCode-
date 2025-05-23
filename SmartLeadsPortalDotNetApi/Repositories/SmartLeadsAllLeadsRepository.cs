using System;
using System.Linq.Expressions;
using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;
using SmartLeadsPortalDotNetApi.Services.Model;

namespace SmartLeadsPortalDotNetApi.Repositories;

public class SmartLeadsAllLeadsRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly SmartleadCampaignRepository smartleadCampaignRepository;
    private readonly ILogger<SmartLeadsAllLeadsRepository> logger;

    public SmartLeadsAllLeadsRepository(DbConnectionFactory dbConnectionFactory, SmartleadCampaignRepository smartleadCampaignRepository, ILogger<SmartLeadsAllLeadsRepository> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        this.smartleadCampaignRepository = smartleadCampaignRepository;
        this.logger = logger;
    }

    public async Task UpsertLeadFromEmailSent(EmailSentPayload payload)
    {
        this.logger.LogInformation("Start UpsertLeadFromEmailSent");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        using var connection = _dbConnectionFactory.GetSqlConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var campaignBdr = await smartleadCampaignRepository.GetCampaignBdr(payload.campaign_id);

        using var transaction = await connection.BeginTransactionAsync();
        try
        {

            var (firstName, lastName) = SplitNameByLastSpace(payload.to_name);
            var lead = new
            {
                LeadId = payload.sl_email_lead_id,
                Email = payload.to_email,
                CampaignId = payload.campaign_id,
                TimeSent = payload.time_sent,
                FirstName = firstName,
                LastName = lastName,
                Bdr = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
                CreatedBy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
                QABy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
            };

            var upsert = """
                MERGE INTO SmartLeadAllLeads WITH (ROWLOCK) AS target
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
                    INSERT (LeadId, Email, CampaignId, CreatedAt, LeadStatus, FirstName, LastName, Bdr, CreatedBy, QABy)
                    VALUES (source.LeadId, source.Email, source.CampaignId, @TimeSent, 'INPROGRESS', @FirstName, @LastName, @Bdr, @CreatedBy, @QABy);
            """;

            await connection.ExecuteAsync(upsert, lead, transaction);
            await transaction.CommitAsync();
            this.logger.LogInformation("UpsertLeadFromEmailSent took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            this.logger.LogError(ex, "Error on UpsertLeadFromEmailSent");
            throw;
        }
    }

    public async Task UpsertLeadFromEmailLinkClick(EmailLinkClickedPayload payload)
    {
        this.logger.LogInformation("Start UpsertLeadFromEmailLinkClick");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        using var connection = _dbConnectionFactory.GetSqlConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var campaignBdr = await smartleadCampaignRepository.GetCampaignBdr(payload.campaign_id.Value);

        using var transaction = await connection.BeginTransactionAsync();
        try
        {

            var (firstName, lastName) = SplitNameByLastSpace(payload.to_name);
            var lead = new
            {
                LeadId = payload.sl_email_lead_id,
                Email = payload.to_email,
                CampaignId = payload.campaign_id,
                FirstName = firstName,
                LastName = lastName,
                Bdr = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
                CreatedBy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
                QABy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
            };

            var upsert = """
                MERGE INTO SmartLeadAllLeads WITH (ROWLOCK) AS target
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
                    INSERT (LeadId, Email, CampaignId, CreatedAt, LeadStatus, FirstName, LastName, Bdr, CreatedBy, QABy)
                    VALUES (source.LeadId, source.Email, source.CampaignId, GETDATE(), 'INPROGRESS', @FirstName, @LastName, @Bdr, @CreatedBy, @QABy);
            """;

            await connection.ExecuteAsync(upsert, lead, transaction);
            await transaction.CommitAsync();
            this.logger.LogInformation("UpsertLeadFromEmailLinkClick took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            this.logger.LogError(ex, "Error on UpsertLeadFromEmailLinkClick");
            throw;
        }
    }

    private (string firstName, string lastName) SplitNameByLastSpace(string fullName)
    {
        // Trim the input to remove leading/trailing spaces
        fullName = fullName?.Trim();

        // Validate input
        if (string.IsNullOrEmpty(fullName))
        {
            throw new ArgumentException("Full name cannot be null or empty.");
        }

        // Find the index of the last space
        int lastSpaceIndex = fullName.LastIndexOf(' ');

        // If no space is found, treat the entire string as the first name
        if (lastSpaceIndex == -1)
        {
            return (fullName, string.Empty);
        }

        // Extract first name and last name
        string firstName = fullName.Substring(0, lastSpaceIndex).Trim();
        string lastName = fullName.Substring(lastSpaceIndex + 1).Trim();

        return (firstName, lastName);
    }
}
