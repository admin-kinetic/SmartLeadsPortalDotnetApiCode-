using Common.Database;
using Dapper;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReprocessBdrAssignment
{
    public class ReprocessBdrAssignmentService
    {
        private readonly DbConnectionFactory dbConnectionFactory;

        public ReprocessBdrAssignmentService(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task Run()
        {
            await this.ReprocessBdrAssignment();
        }

        private async Task ReprocessBdrAssignment()
        {
            using var connection = this.dbConnectionFactory.CreateConnection();
            var update = """
                    Update slal
                	    SET 
                		    slal.BDR = slc.Bdr,
                		    slal.CreatedBy = slc.Bdr,
                		    slal.QABy = slc.Bdr
                    From SmartLeadAllLeads  slal
                    Inner Join SmartleadCampaigns slc On slc.Id = slal.CampaignId
                    Inner Join SmartleadsAccountCampaigns slac On slac.CampaignId = slal.CampaignId
                    Inner Join SmartleadsAccounts sla On sla.Id = slac.SmartleadsAccountId
                    Where slal.BDR IS NOT NULL AND slc.Bdr IS NOT NULL AND sla.id = 1
                """;
            await connection.ExecuteAsync(update);
        }
    }
}
