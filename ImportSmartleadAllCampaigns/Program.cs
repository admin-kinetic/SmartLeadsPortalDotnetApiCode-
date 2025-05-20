using Common.Database;
using Common.Services;
using Microsoft.Extensions.Configuration;

namespace ImportSmartleadAllCampaigns
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var dbConnectionFactory = new DbConnectionFactory(configuration);
            var smartleadHttpService = new SmartLeadHttpService();
            var service = new SmartleadAllCampaignsService(smartleadHttpService, dbConnectionFactory, configuration);
            await service.Run();
        }
    }
}
