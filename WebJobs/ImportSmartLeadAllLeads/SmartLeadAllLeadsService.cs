using System;
using System.Transactions;
using Dapper;
using ExportedLeadsFromLeadsPortal;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using SmartLeadsAllLeadsToPortal.Entities;

namespace SmartLeadsAllLeadsToPortal;

public class SmartLeadAllLeadsService
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly SmartLeadHttpService smartLeadHttpService;

    public SmartLeadAllLeadsService(DbConnectionFactory dbConnectionFactory, SmartLeadHttpService smartLeadHttpService)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.smartLeadHttpService = smartLeadHttpService;
    }

    public async Task SaveAllLeads()
    {
        var date = DateTime.Now.AddDays(-1);
        var limit = 100;
        var offset = 100;
        var hasMore = false;
        do
        {
            var allLeads = await this.smartLeadHttpService.GetAllLeads(date, offset, limit);
            hasMore = allLeads.hasMore;

            var smartLeadAllLeads = allLeads.data.Select(lead => new SmartLeadAllLeads
            {
                LeadId = int.Parse(lead.id),
                Email = lead.email,
                CampaignId = lead.campaigns.First()?.campaign_id,
                FirstName = lead.first_name,
                LastName = lead.last_name,
                CreatedAt = lead.created_at,
                PhoneNumber = lead.phone_number,
                CompanyName = lead.company_name,
                LeadStatus = lead.campaigns.First()?.lead_status
            });

            using (var connection = this.dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var insert = """
                        INSERT INTO SmartLeadAllLeads (LeadId, Email, CampaignId, FirstName, LastName, CreatedAt, PhoneNumber, CompanyName, LeadStatus)
                        VALUES (@LeadId, @Email, @CampaignId, @FirstName, @LastName, @CreatedAt, @PhoneNumber, @CompanyName, @LeadStatus)
                    """;

                        await connection.ExecuteAsync(insert, smartLeadAllLeads, transaction);

                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            offset += limit;
        } while (hasMore);


    }
}
