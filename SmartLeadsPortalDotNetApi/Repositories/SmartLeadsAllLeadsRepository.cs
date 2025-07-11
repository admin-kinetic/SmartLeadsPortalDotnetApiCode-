using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;
using System.Data;

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
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        this.logger.LogInformation("Start UpsertLeadFromEmailSent");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var connection = await _dbConnectionFactory.GetSqlConnectionAsync();

        var campaignBdr = await smartleadCampaignRepository.GetCampaignBdr(payload.campaign_id ?? 0);
        var (firstName, lastName) = SplitNameByLastSpace(payload.to_name ?? string.Empty);

        var lead = new
        {
            LeadId = double.Parse(payload.sl_email_lead_id ?? "0"),
            Email = payload.to_email ?? string.Empty,
            CampaignId = payload.campaign_id,
            TimeSent = payload.time_sent,
            FirstName = firstName,
            LastName = lastName,
            Bdr = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
            CreatedBy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
            QABy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
        };

        // First check if the lead exists
        var selectQuery = """
            SELECT LeadId 
            FROM SmartLeadAllLeads
            WHERE LeadId = @LeadId
        """;

        var existingLead = await connection.QueryFirstOrDefaultAsync<dynamic>(selectQuery, new { lead.LeadId });

        if (existingLead != null)
        {
            // Update existing lead
            var updateQuery = """
                UPDATE SmartLeadAllLeads
                SET Email = @Email,
                    CampaignId = @CampaignId
                WHERE LeadId = @LeadId
            """;

            await connection.ExecuteAsync(updateQuery, lead);
        }
        else
        {
            // Insert new lead
            var insertQuery = """
                INSERT INTO SmartLeadAllLeads 
                (LeadId, Email, CampaignId, CreatedAt, LeadStatus, FirstName, LastName, Bdr, CreatedBy, QABy)
                VALUES 
                (@LeadId, @Email, @CampaignId, @TimeSent, 'INPROGRESS', @FirstName, @LastName, @Bdr, @CreatedBy, @QABy)
            """;

            await connection.ExecuteAsync(insertQuery, lead);
        }

        this.logger.LogInformation("UpsertLeadFromEmailSent took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
    }

    public async Task UpsertLeadFromEmailLinkClick(EmailLinkClickedPayload payload)
    {
        this.logger.LogInformation("Start UpsertLeadFromEmailLinkClick");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var connection = await _dbConnectionFactory.GetSqlConnectionAsync();

        var campaignBdr = await smartleadCampaignRepository.GetCampaignBdr(payload.campaign_id ?? 0);
        var (firstName, lastName) = SplitNameByLastSpace(payload.to_name ?? string.Empty);

        var lead = new
        {
            LeadId = double.Parse(payload.sl_email_lead_id ?? "0"),
            Email = payload.to_email ?? string.Empty,
            CampaignId = payload.campaign_id,
            FirstName = firstName,
            LastName = lastName,
            Bdr = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
            CreatedBy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
            QABy = string.Compare(campaignBdr, "Steph", StringComparison.OrdinalIgnoreCase) == 0 ? string.Empty : campaignBdr,
        };

        // First check if the lead exists
        var selectQuery = """
            SELECT LeadId 
            FROM SmartLeadAllLeads
            WHERE LeadId = @LeadId
        """;

        var existingLead = await connection.QueryFirstOrDefaultAsync<dynamic>(selectQuery, new { lead.LeadId });

        if (existingLead != null)
        {
            // Update existing lead
            var updateQuery = """
                UPDATE SmartLeadAllLeads
                SET Email = @Email,
                    CampaignId = @CampaignId
                WHERE LeadId = @LeadId
            """;

            await connection.ExecuteAsync(updateQuery, lead);
        }
        else
        {
            // Insert new lead
            var insertQuery = """
                INSERT INTO SmartLeadAllLeads 
                (LeadId, Email, CampaignId, CreatedAt, LeadStatus, FirstName, LastName, Bdr, CreatedBy, QABy)
                VALUES 
                (@LeadId, @Email, @CampaignId, GETDATE(), 'INPROGRESS', @FirstName, @LastName, @Bdr, @CreatedBy, @QABy)
            """;

            await connection.ExecuteAsync(insertQuery, lead);
        }
        this.logger.LogInformation("UpsertLeadFromEmailLinkClick took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
    }

    public async Task<SmartLeadAllLeads?> GetByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        using var connection = _dbConnectionFactory.GetSqlConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var query = """
                SELECT * FROM SmartLeadAllLeads WHERE Email = @email
            """;

        return await connection.QueryFirstOrDefaultAsync<SmartLeadAllLeads?>(query, new { email });
    }

    public async Task InsertLeadFromSmartleads(SmartLeadsByEmailResponse leadFromSmartLeads)
    {
        if (leadFromSmartLeads == null)
        {
            throw new ArgumentNullException(nameof(leadFromSmartLeads));
        }

        var newLead = new SmartLeadAllLeads
        {
            LeadId = double.Parse(leadFromSmartLeads.id ?? "0"),
            CampaignId = leadFromSmartLeads.lead_campaign_data?.FirstOrDefault()?.campaign_id,
            FirstName = leadFromSmartLeads.first_name,
            LastName = leadFromSmartLeads.last_name,
            CreatedAt = leadFromSmartLeads.created_at,
            PhoneNumber = leadFromSmartLeads.phone_number,
            CompanyName = leadFromSmartLeads.company_name,
            LeadStatus = "INPROGRESS",
            Email = leadFromSmartLeads.email,
            BDR = leadFromSmartLeads.custom_fields?.BDR,
            CreatedBy = leadFromSmartLeads.custom_fields?.Created_by,
            QABy = leadFromSmartLeads.custom_fields?.QA_by,
            Location = leadFromSmartLeads.location
        };
        using var connection = _dbConnectionFactory.GetSqlConnection();
        var insert = """
                INSERT INTO SmartLeadAllLeads (LeadId, Email, CampaignId, CreatedAt, FirstName, LastName, PhoneNumber, CompanyName, LeadStatus, Location, Bdr, CreatedBy, QABy)
                VALUES (@LeadId, @Email, @CampaignId, @CreatedAt, @FirstName, @LastName, @PhoneNumber, @CompanyName, @LeadStatus, @Location, @BDR, @CreatedBy, @QABy)
            """;
        await connection.ExecuteAsync(insert, newLead);
    }

    private (string firstName, string lastName) SplitNameByLastSpace(string fullName)
    {
        // If input is null or empty, return empty strings
        if (string.IsNullOrEmpty(fullName))
        {
            return (string.Empty, string.Empty);
        }

        // Trim the input to remove leading/trailing spaces
        fullName = fullName.Trim();

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

    public async Task UpdateLeadCategory(string email, string category)
    {
        try
        {
            var lead = await this.GetByEmail(email);
            if (lead == null)
            {
                throw new ArgumentException("Email not found in leads.");
            }

            using var connection = _dbConnectionFactory.GetSqlConnection();
            var update = """
                    UPDATE SmartLeadAllLeads 
                    SET SmartLeadCategory = @category
                    WHERE Email = @email
                """;
            await connection.ExecuteAsync(update, new { email, category });
        }
        catch (System.Exception ex)
        {
            this.logger.LogError("Database error: {0}", ex.Message);
            throw;
        }
    }

    public async Task<List<string>> GetLeadGen()
    {
        using var connection = _dbConnectionFactory.GetSqlConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var query = """
                SELECT DISTINCT CreatedBy FROM SmartLeadAllLeads WHERE CreatedBy IS NOT NULL AND CreatedBy != ''
            """;
        var queryResult = await connection.QueryAsync<string>(query);
        return queryResult.ToList();
    }

    public async Task<List<string>> GetQa()
    {
        using var connection = _dbConnectionFactory.GetSqlConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var query = """
                SELECT DISTINCT QaBy FROM SmartLeadAllLeads WHERE QaBy IS NOT NULL AND QaBy != ''
            """;
        var queryResult = await connection.QueryAsync<string>(query);
        return queryResult.ToList();
    }

    public async Task<List<string>> GetBdr()
    {
        using var connection = _dbConnectionFactory.GetSqlConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var query = """
                SELECT DISTINCT Bdr FROM SmartLeadAllLeads WHERE Bdr IS NOT NULL AND Bdr != ''
            """;
        var queryResult = await connection.QueryAsync<string>(query);
        return queryResult.ToList();
    }

    public async Task<int> UpSertPhonenumbersInAllLeads(SmartLeadsUpdatePhoneNumberRequest filter)
    {
        try
        {
            using (var connection = this._dbConnectionFactory.GetSqlConnection())
            {
                var countProcedure = "sm_spUpSertPhonenumbersInAllLeads";
                var param = new DynamicParameters();
                param.Add("@email", filter.Email);
                param.Add("@phonenumber", filter.PhoneNumber);
                var affectedRows = await connection.ExecuteAsync(countProcedure, param, commandType: CommandType.StoredProcedure);

                return affectedRows;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating phone numbers in database.", ex);
        }
    }
    
     public async Task<string?> GetLeadStatus(string email)
    {
        try
        {
            await using var connection = await _dbConnectionFactory.GetSqlConnectionAsync();
            var query = """
                    SELECT LeadStatus FROM SmartLeadAllLeads WHERE Email = @email
                """;
            return await connection.QueryFirstOrDefaultAsync<string?>(query, new { email });
        }
        catch (Exception ex)
        {
            logger.LogError("Database error: {0}", ex.Message);
            throw;
        }
    }
}
