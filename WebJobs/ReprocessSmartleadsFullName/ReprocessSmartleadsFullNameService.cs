using Common.Database;
using Common.Services;
using Dapper;
using Microsoft.Extensions.Logging;
using ReprocessSmartleadsFullName.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReprocessSmartleadsFullName
{
    internal class ReprocessSmartleadsFullNameService
    {
        private readonly DbConnectionFactory dbConnectionFactory;
        private readonly SmartLeadHttpService smartLeadHttpService;
        private readonly ILogger<ReprocessSmartleadsFullNameService> logger;

        public ReprocessSmartleadsFullNameService(
            DbConnectionFactory dbConnectionFactory, 
            SmartLeadHttpService smartLeadHttpService, 
            ILogger<ReprocessSmartleadsFullNameService> logger)
        {
            this.dbConnectionFactory = dbConnectionFactory;
            this.smartLeadHttpService = smartLeadHttpService;
            this.logger = logger;
        }

        public async Task Run()
        {
            using var connection = dbConnectionFactory.CreateConnection();
            var queryLeadsToBeUpdated = """
                    Select sal.Email, sa.Id as AccountId, sa.Name as AccoutName, sa.ApiKey as AccountApiKey From SmartLeadAllLeads sal
                    INNER JOIN SmartleadsAccountCampaigns sac ON sac.CampaignId = sal.CampaignId
                    INNER JOIN SmartleadsAccounts sa ON sa.Id = sac.SmartleadsAccountId
                    WHERE FirstName IS NULL AND LastName IS NULL
                    ORDER BY CreatedAt Desc
                """;

            var leadsToBeUpdated = await connection.QueryAsync<LeadsToBeUpdated>(queryLeadsToBeUpdated);
            this.logger.LogInformation($"Found {leadsToBeUpdated.Count()} leads to update");
            foreach (var lead in leadsToBeUpdated)
            {
                if (string.IsNullOrEmpty(lead.Email))
                {
                    continue;
                }

                await Task.Delay(100);
                var response = await DbExecution.ExecuteWithRetryAsync(async () =>
                {
                    return await smartLeadHttpService.LeadByEmail(lead.Email, lead.AccountApiKey);
                });
                if (response == null)
                {
                    continue;
                }

                this.logger.LogInformation($"Updated lead {lead.Email} with name {response.first_name} {response.last_name}");

                var updateQuery = """
                        UPDATE SmartLeadAllLeads SET 
                            FirstName = @FirstName, 
                            LastName = @LastName
                        WHERE Email = @Email
                    """;

                await connection.ExecuteAsync(updateQuery, new
                {
                    FirstName = response.first_name,
                    LastName = response.last_name,
                    Email = lead.Email
                });
            }
        }
    }
}
