using Common.Database;
using Common.Services;
using ImportSmartLeadStatistics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

// Configure logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

// Create logger factory
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSerilog();
});

var logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation("Starting SmartLead Statistics Import...");

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
var dbConnectionFactory = new DbConnectionFactory(configuration);

var smartLeadHttpService = new SmartLeadHttpService();

// var statistics = await smartLeadHttpService.GetCampaignStatistics(DateTime.Now.AddDays(-1), 0, 100);

var smartLeadCampaignStatisticsService = new SmartLeadCampaignStatisticsService(dbConnectionFactory, smartLeadHttpService, configuration, loggerFactory.CreateLogger<SmartLeadCampaignStatisticsService>());

//await smartLeadCampaignStatisticsService.SaveAllCampaignStatisticsByCampaignId();
await smartLeadCampaignStatisticsService.SaveAllCampaignLeadStatistics();

logger.LogInformation("Done SmartLead Statistics Import.");