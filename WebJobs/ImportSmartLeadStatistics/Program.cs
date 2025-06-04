using Common.Database;
using Common.Services;
using ImportSmartLeadStatistics;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Starting SmartLead Statistics Import...");

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
var dbConnectionFactory = new DbConnectionFactory(configuration);

var smartLeadHttpService = new SmartLeadHttpService();

// var statistics = await smartLeadHttpService.GetCampaignStatistics(DateTime.Now.AddDays(-1), 0, 100);

var smartLeadCampaignStatisticsService = new SmartLeadCampaignStatisticsService(dbConnectionFactory, smartLeadHttpService, configuration);

//await smartLeadCampaignStatisticsService.SaveAllCampaignStatisticsByCampaignId();
await smartLeadCampaignStatisticsService.SaveAllCampaignLeadStatistics();

Console.WriteLine("Done SmartLead Statistics Import.");