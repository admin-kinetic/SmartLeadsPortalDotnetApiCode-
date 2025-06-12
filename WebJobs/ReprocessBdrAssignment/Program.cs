using Common.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading.Tasks;

namespace ReprocessBdrAssignment
{
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

            // Create logger
            var logger = loggerFactory.CreateLogger<Program>();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();
            var dbConnectionFactory = new DbConnectionFactory(configuration);
            var service = new ReprocessBdrAssignmentService(dbConnectionFactory);
            await service.Run();
        }
    }
}
