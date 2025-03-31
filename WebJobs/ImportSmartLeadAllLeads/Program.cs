
using Common.Services;
using ExportedLeadsFromLeadsPortal;
using Microsoft.Extensions.Configuration;
using SmartLeadsAllLeadsToPortal;

Console.WriteLine("Importing Smartlead All Leads to Portal");

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
var dbConnectionFactory = new DbConnectionFactory(configuration);

var smartLeadHttpService = new SmartLeadHttpService();
// var fromDate = DateTime.Now.AddDays(-1);
// var allLeads = await smartLeadHttpService.GetAllLeads(fromDate, 0);

var smartLeadAllLeadsService = new SmartLeadAllLeadsService(dbConnectionFactory, smartLeadHttpService);
// await smartLeadAllLeadsService.SaveAllLeads();
await smartLeadAllLeadsService.SaveAllLeadsByEmail();

Console.WriteLine("Done importing Smartlead All Leads to Portal");