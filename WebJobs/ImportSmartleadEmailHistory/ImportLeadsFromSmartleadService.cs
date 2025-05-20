using Common.Repositories;
using Common.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportSmartleadEmailHistory
{
    public class ImportLeadsFromSmartleadService
    {
        private readonly SmartLeadHttpService smartLeadHttpService;
        private readonly SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository;
        private readonly IConfiguration _configuration;

        private readonly string botSmartleadApiKey = "0f47052b-e08b-488b-8ec3-dd949eec520a_umaccv1";

        public ImportLeadsFromSmartleadService(SmartLeadHttpService smartLeadHttpService, SmartLeadsExportedContactsRepository smartLeadsExportedContactsRepository, IConfiguration configuration)
        {
            this.smartLeadHttpService = smartLeadHttpService;
            this.smartLeadsExportedContactsRepository = smartLeadsExportedContactsRepository;
            _configuration = configuration;
        }

        public async Task Run()
        {
            Console.WriteLine("Start getting leads from smartlead...");
            var exportedContacts = await this.smartLeadsExportedContactsRepository.GetExportedContacts();

            if(!exportedContacts.Any())
            {
                Console.WriteLine($"No more leads from smartlead to process.");
                return;
            }

            Console.WriteLine($"Found {exportedContacts.Count()} leads from smartlead.");

            foreach (var exportedContact in exportedContacts)
            {
                await Task.Delay(1000);
                var lead = await this.smartLeadHttpService.LeadByEmail(exportedContact.Email, botSmartleadApiKey);

                if(lead == null)
                {
                    await this.smartLeadsExportedContactsRepository.RemovedFromSmartlead(exportedContact.Email);
                    Console.WriteLine($"Lead with email {exportedContact.Email} no longer exists.");
                    continue;
                }

                var leadCampaignId = lead?.lead_campaign_data[0]?.campaign_id;

                if(leadCampaignId == null)
                {
                    Console.WriteLine($"Lead with email {exportedContact.Email} has no campaign id.");
                    continue;
                }

                var messagesByLead = await this.smartLeadHttpService.MessageHistoryByLead(leadCampaignId, lead.id, botSmartleadApiKey);
                if (!messagesByLead.history.Any())
                {
                    Console.WriteLine($"Lead with email {exportedContact.Email} has no messages history.");
                    continue;
                }

                await this.smartLeadsExportedContactsRepository.UpdateHasReply(lead.email, messagesByLead.history);

                Console.WriteLine($"Lead with email {exportedContact.Email} has been updated.");
                Console.WriteLine($"Next email...");
            }

            Console.WriteLine("Done a batch of leads from smartlead.");
        }
    }
}
