using Common.Database;
using Common.Models.Webhooks;
using Common.Repositories;
using Common.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReprocessLeadCategoryUpdated;

public class ReprocessLeadCategoryUpdatedService
{
    private readonly IConfiguration _configuration;
    private readonly DbConnectionFactory _dbConnectionFactory;
    private readonly SmartLeadsExportedContactsRepository _smartLeadsExportedContactsRepository;
    private readonly SmartLeadsAllLeadsRepository smartLeadsAllLeadsRepository;
    private readonly SmartleadCampaignRepository smartleadCampaignRepository;
    private readonly SmartLeadHttpService _smartLeadHttpService;

    public ReprocessLeadCategoryUpdatedService(
        IConfiguration configuration,
        DbConnectionFactory dbConnectionFactory, 
        SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository,
        SmartLeadsAllLeadsRepository smartLeadsAllLeadsRepository,
        SmartleadCampaignRepository smartleadCampaignRepository,
        SmartLeadHttpService smartLeadHttpService)
    {
        _configuration = configuration;
        _dbConnectionFactory = dbConnectionFactory;
        _smartLeadsExportedContactsRepository = smartLeadsExportedContactsRepository;
        this.smartLeadsAllLeadsRepository = smartLeadsAllLeadsRepository;
        this.smartleadCampaignRepository = smartleadCampaignRepository;
        _smartLeadHttpService = smartLeadHttpService;
    }

    public async Task Run()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        var dayOffset = _configuration.GetSection("DaysOffset").Get<int?>();
        var dateFrom = _configuration.GetSection("DateFrom").Get<DateTime?>();
        var dateTo = _configuration.GetSection("DateTo").Get<DateTime?>();

        string webhookQuery = string.Empty;

        if(dayOffset != null)
        {
            webhookQuery = """
                Select Request
                From Webhooks 
                Where (EventType = 'LEAD_CATEGORY_UPDATED') 
                    AND CONVERT(date, CreatedAt) >= CONVERT(DATE, DATEADD(DAY, -@dayOffset, GETDATE()))
                Order By CreatedAt DESC
            """;
        }

        if (dateFrom != null && dateTo != null)
        {
            webhookQuery = """
                Select Request
                From Webhooks 
                Where (EventType = 'LEAD_CATEGORY_UPDATED') 
                    AND CONVERT(date, CreatedAt) >= @dateFrom
                    AND CONVERT(date, CreatedAt) <= @dateTo
                Order By CreatedAt DESC
            """;
        }

        if (string.IsNullOrEmpty(webhookQuery))
        {
            throw new Exception("Invalid setting occured!");
        }

        var webhooks = await connection.QueryAsync<string>(webhookQuery, new { dayOffset, dateFrom, dateTo });
        foreach (var webhook in webhooks)
        {
            var payloadObject = JsonSerializer.Deserialize<LeadCategoryUpdatePayload>(webhook);
            var email = payloadObject.lead_email;

            if (string.IsNullOrWhiteSpace(email.ToString()))
            {
                throw new ArgumentNullException("to_email", "Email is required.");
            }

            var lead = await this.smartLeadsAllLeadsRepository.GetByEmail(email.ToString());

            if (lead == null)
            {
                Console.WriteLine($"No lead found for {email.ToString()} email");
                var account = await this.smartleadCampaignRepository.GetAccountByCampaignId(payloadObject.campaign_id);
                var leadFromSmartLeads = await _smartLeadHttpService.LeadByEmail(email.ToString(), account.ApiKey);

               
                await this.smartLeadsAllLeadsRepository.InsertLeadFromSmartleads(leadFromSmartLeads);
                lead = await this.smartLeadsAllLeadsRepository.GetByEmail(email.ToString());
            }

            Console.WriteLine($"Update lead category for {email.ToString()} email");

            var leadCategoryName = payloadObject.lead_category.new_name;

            //await _smartLeadsExportedContactsRepository.UpdateLeadCategory(email.ToString(), leadCategoryName.ToString());
            await this.smartLeadsAllLeadsRepository.UpdateLeadCategory(email.ToString(), leadCategoryName.ToString());
        }
    }
}
