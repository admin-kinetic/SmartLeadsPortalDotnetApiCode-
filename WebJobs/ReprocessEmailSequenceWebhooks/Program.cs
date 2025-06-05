using Common.Database;
using Common.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ReprocessEmailSequenceWebhooks
{
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

            // Create logger instance
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Start reprocessing email sequence webhooks...");

            var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json").Build();
            var dbConnectionFactory = new DbConnectionFactory(configuration);
            var messageHistoryRepository = new MessageHistoryRepository(dbConnectionFactory, loggerFactory.CreateLogger<MessageHistoryRepository>());
            var smartLeadsEmailStatisticsRepository = new SmartLeadsEmailStatisticsRepository(dbConnectionFactory, loggerFactory.CreateLogger<SmartLeadsEmailStatisticsRepository>());
            var appService = new ReprocessService(configuration, loggerFactory.CreateLogger<ReprocessService>(), dbConnectionFactory, messageHistoryRepository, smartLeadsEmailStatisticsRepository);
            await appService.Run();
            logger.LogInformation("Done reprocessing email sequence webhooks...");
        }
    }
}
