using Common.Database;
using Common.Services;
using Microsoft.Extensions.Configuration;

namespace ReprocessEmailStatistics
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start reprocessing email with no sent details.");

            var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json").Build();
            var dbConnectionFactory = new DbConnectionFactory(configuration);

            var smartLeadHttpService = new SmartLeadHttpService();

            var service = new ReprocessEmailStatisticsService(dbConnectionFactory, smartLeadHttpService);
            await service.Run();

            Console.WriteLine("Done reprocessing email with no sent details.");
        }
    }
}
