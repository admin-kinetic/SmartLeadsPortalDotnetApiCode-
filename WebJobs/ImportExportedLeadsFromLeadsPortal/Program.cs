using Common.Database;
using Microsoft.Extensions.Configuration;

namespace ImportExportedLeadsFromLeadsPortal;

public class Program
{
    static async Task Main(string[] args)
    {

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        var dbConnectionFactory = new DbConnectionFactory(configuration);
        var leadsPortalService = new LeadsPortalService();
        var smartLeadsExportedContactsService = new SmartLeadsExportedContactsService(dbConnectionFactory, leadsPortalService);
        var fromDate = DateTime.Now.AddDays(-1);
        var toDate = DateTime.Now;
        await smartLeadsExportedContactsService.SaveExportedContacts(fromDate, toDate);

        // var appService = new AppService();
        // appService.Run();
    }
}

