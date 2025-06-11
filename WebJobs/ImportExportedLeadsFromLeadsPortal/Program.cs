using Common.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ImportExportedLeadsFromLeadsPortal;

public class Program
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

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        var dbConnectionFactory = new DbConnectionFactory(configuration);
        var leadsPortalService = new LeadsPortalService();
        var smartLeadsExportedContactsService = new SmartLeadsExportedContactsService(
            dbConnectionFactory, leadsPortalService, loggerFactory.CreateLogger<SmartLeadsExportedContactsService>());
        var fromDate = DateTime.Now.AddDays(-2);
        var toDate = DateTime.Now.AddDays(-1);
        await smartLeadsExportedContactsService.SaveExportedContacts(fromDate, toDate);

        // var appService = new AppService();
        // appService.Run();
    }
}

