using Common.Database;
using Common.Services;
using Microsoft.Extensions.Configuration;

namespace ReprocessEmailSentWebhook;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Start reprocessing EMAIL_SENT webhook...");

        var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json").Build();
        var dbConnectionFactory = new DbConnectionFactory(configuration);

        var smartLeadHttpService = new SmartLeadHttpService();

        // var statistics = await smartLeadHttpService.GetCampaignStatistics(DateTime.Now.AddDays(-1), 0, 100);

        var service = new ReprocessEmailSentWebhookService(dbConnectionFactory);

        await service.Run();

        Console.WriteLine("Done reprocessing EMAIL_SENT webhook...");
    }
}
