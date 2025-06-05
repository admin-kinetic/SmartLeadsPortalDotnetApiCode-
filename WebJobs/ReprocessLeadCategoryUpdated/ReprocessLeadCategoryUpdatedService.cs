using Common.Database;
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
    private readonly SmartLeadHttpService _smartLeadHttpService;

    public ReprocessLeadCategoryUpdatedService(
        IConfiguration configuration,
        DbConnectionFactory dbConnectionFactory, 
        SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository,
        SmartLeadHttpService smartLeadHttpService)
    {
        _configuration = configuration;
        _dbConnectionFactory = dbConnectionFactory;
        _smartLeadsExportedContactsRepository = smartLeadsExportedContactsRepository;
        _smartLeadHttpService = smartLeadHttpService;
    }

    public async Task Run()
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        var dayOffset = _configuration.GetSection("DaysOffset").Get<int>();

        var webhookQuery = """
                Select Request
                From Webhooks 
                Where (EventType = 'LEAD_CATEGORY_UPDATED') AND CONVERT(date, CreatedAt) >= CONVERT(DATE, DATEADD(DAY, -@dayOffset, GETDATE()))
                Order By CreatedAt ASC
            """;

        var webhooks = await connection.QueryAsync<string>(webhookQuery, new { dayOffset });
        foreach (var payloadObject in webhooks.Select(w => JsonSerializer.Deserialize<JsonElement>(w)))
        {
            var email = payloadObject.GetProperty("lead_email");

            if (string.IsNullOrWhiteSpace(email.ToString()))
            {
                throw new ArgumentNullException("to_email", "Email is required.");
            }

            var lead = _smartLeadsExportedContactsRepository.GetLeadByEmail(email.ToString());

            if (lead == null)
            {
                Console.WriteLine($"No lead found for {email.ToString()} email");
                var leadFromSmartLeads = await _smartLeadHttpService.LeadByEmail(email.ToString(), string.Empty);
                continue;
            }

            Console.WriteLine($"Update lead category for {email.ToString()} email");

            var leadCategoryName = payloadObject.GetProperty("lead_category").GetProperty("new_name");

            await _smartLeadsExportedContactsRepository.UpdateLeadCategory(email.ToString(), leadCategoryName.ToString());
        }
    }
}
