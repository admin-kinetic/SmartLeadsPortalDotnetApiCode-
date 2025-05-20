using Common.Database;
using Common.Repositories;
using Common.Services;
using Microsoft.Extensions.Configuration;

namespace ImportSmartleadEmailHistory
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var dbConnectionFactory = new DbConnectionFactory(config);
            var smartLeadsExportedContactsRepository = new SmartLeadsExportedContactsRepository(dbConnectionFactory);
            var smartleadHttpService = new SmartLeadHttpService();
            var service = new ImportLeadsFromSmartleadService(smartleadHttpService, smartLeadsExportedContactsRepository, config);
            await service.Run();
        }
    }
}
