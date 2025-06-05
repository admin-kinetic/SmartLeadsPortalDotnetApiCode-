using Common.Database;
using Common.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ImportSmartleadAllCampaigns
{
    public class SmartleadAllCampaignsService
    {
        private readonly SmartLeadHttpService _smartLeadHttpService;
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly IConfiguration _configuration;

        public SmartleadAllCampaignsService(SmartLeadHttpService smartLeadHttpService, DbConnectionFactory dbConnectionFactory, IConfiguration configuration)
        {
            _smartLeadHttpService = smartLeadHttpService;
            _dbConnectionFactory = dbConnectionFactory;
            _configuration = configuration;
        }

        public async Task Run()
        {
            var apiKeys = _configuration.GetSection("ApiKeys").Get<string[]>() ?? Array.Empty<string>();

            foreach (var apiKey in apiKeys)
            {
                var campaigns = await _smartLeadHttpService.ListAllCampaigns(apiKey);
                var campaignsToUpsert = campaigns
                    .Select(x => new { 
                        id = x.id, 
                        x.name, 
                        x.status, 
                        bdr = CampaignToBdrMapping.FirstOrDefault(name => x.name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    });
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var upsert = """
                                MERGE SmartLeadCampaigns AS target
                                USING (SELECT @Id AS Id) AS source
                                ON target.Id = source.Id
                                WHEN MATCHED THEN
                                    UPDATE SET
                                        target.Name = @Name,
                                        target.Status = @Status,
                                        target.Bdr = @Bdr
                                WHEN NOT MATCHED THEN
                                    INSERT (Id, Name, Status, Bdr)
                                    VALUES (source.Id, @Name, @Status, @Bdr);
                                """;
                            await connection.ExecuteAsync(upsert, campaignsToUpsert, transaction: transaction);
                            Console.WriteLine($"Upserted {campaignsToUpsert.Count()}");
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Failed to upsert {campaignsToUpsert.Count()}");
                            transaction.Rollback();
                            throw;
                        }
                    }   
                }
            }
        }

        private List<string> CampaignToBdrMapping = new List<string>() {
            "Amirah",
            "Cherrie",
            "Dondi",
            "Elzon",
            "Ivan",
            "Joan",
            "Kat",
            "Leslee",
            "Marie",
            "Pat",
            "Ria",
            "Ribhi",
            "Steph"
        };
    }
}
