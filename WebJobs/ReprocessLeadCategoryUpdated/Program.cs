using Common.Database;
using Common.Repositories;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ReprocessLeadCategoryUpdated;

internal class Program
{
    static async Task Main(string[] args)
    {

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

        // Create logger
        var logger = loggerFactory.CreateLogger<Program>();


        logger.LogInformation("Reprocessing lead category updated webhook");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        var dbConnectionFactory = new DbConnectionFactory(configuration);
        var smartLeadsExportedContactsRepository = new SmartLeadsExportedContactsRepository(dbConnectionFactory);
        var smartLeadCampainsRepository = new SmartleadCampaignRepository(dbConnectionFactory);
        var smartLeadsAllLeadsRepository = new SmartLeadsAllLeadsRepository(dbConnectionFactory, smartLeadCampainsRepository, loggerFactory.CreateLogger<SmartLeadsAllLeadsRepository>());
        var smartLeadsHttpService = new SmartLeadHttpService();
        var reprocessLeadCategoryUpdatedService = new ReprocessLeadCategoryUpdatedService(
            configuration, dbConnectionFactory, smartLeadsExportedContactsRepository, smartLeadsAllLeadsRepository, smartLeadCampainsRepository, smartLeadsHttpService);
        await reprocessLeadCategoryUpdatedService.Run();
        logger.LogInformation("Done reprocessing lead category updated webhook");
    }
}
