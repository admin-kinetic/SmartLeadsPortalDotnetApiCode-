using Common.Database;
using Common.Repositories;
using Common.Services;
using Microsoft.Extensions.Configuration;

namespace ReprocessLeadCategoryUpdated;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Reprocessing lead category updated webhook");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        var dbConnectionFactory = new DbConnectionFactory(configuration);
        var smartLeadsExportedContactsRepository = new SmartLeadsExportedContactsRepository(dbConnectionFactory);
        var smartLeadsHttpService = new SmartLeadHttpService();
        var reprocessLeadCategoryUpdatedService = new ReprocessLeadCategoryUpdatedService(dbConnectionFactory, smartLeadsExportedContactsRepository, smartLeadsHttpService);
        await reprocessLeadCategoryUpdatedService.Run();
        Console.WriteLine("Done reprocessing lead category updated webhook");
    }
}
