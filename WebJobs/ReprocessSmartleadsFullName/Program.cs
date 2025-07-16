using Common.Database;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ReprocessSmartleadsFullName;

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

        var logger = loggerFactory.CreateLogger<Program>();

        logger.LogInformation("Start reprocess names from smartleads...");

        var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json").Build();
        var dbConnectionFactory = new DbConnectionFactory(configuration);

        var smartLeadHttpService = new SmartLeadHttpService();

        var service = new ReprocessSmartleadsFullNameService(dbConnectionFactory, smartLeadHttpService, loggerFactory.CreateLogger<ReprocessSmartleadsFullNameService>());
        await service.Run();

        logger.LogInformation("Done reprocess names from smartleads...");
    }
}
