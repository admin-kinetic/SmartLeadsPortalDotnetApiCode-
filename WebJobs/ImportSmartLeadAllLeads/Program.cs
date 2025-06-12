
using Common.Database;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using SmartLeadsAllLeadsToPortal;

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

// Create logger
var logger = loggerFactory.CreateLogger<Program>();


Console.WriteLine("Importing Smartlead All Leads to Portal");

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
var dbConnectionFactory = new DbConnectionFactory(configuration);

var smartLeadHttpService = new SmartLeadHttpService();
// var fromDate = DateTime.Now.AddDays(-1);
// var allLeads = await smartLeadHttpService.GetAllLeads(fromDate, 0);

var smartLeadAllLeadsService = new SmartLeadAllLeadsService(dbConnectionFactory, smartLeadHttpService, configuration);
await smartLeadAllLeadsService.SaveAllLeads();
//await smartLeadAllLeadsService.SaveAllLeadsByEmail();

Console.WriteLine("Done importing Smartlead All Leads to Portal");