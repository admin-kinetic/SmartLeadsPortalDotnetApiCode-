using Common.Database;
using Common.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var campaignsToUpsert = campaigns.Select(x => new { id = x.id, x.name, x.status, bdr = this.CampaignToBdrMapping.TryGetValue(x.id.Value, out string bdrMap) ? bdrMap : string.Empty });
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    connection.Open();
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

        private Dictionary<int, string> CampaignToBdrMapping = new Dictionary<int, string>() {
            { 613467, "Steph" },
            { 616195, "Steph" },
            { 747938, "Steph" },
            { 747948, "Steph" },
            { 747963, "Steph" },
            { 857001, "Steph" },
            { 946855, "Steph" },
            { 1673088, "Steph" },
            { 1673218, "Steph" },
            { 1673237, "Steph" },
            { 108832, "Cherrie" },
            { 108835, "Ria" },
            { 108836, "Marie" },
            { 108837, "Leslee" },
            { 108838, "Ribhi" },
            { 108840, "Amirah" },
            { 108848, "Ria" },
            { 108850, "Cherrie" },
            { 108851, "Marie" },
            { 108852, "Leslee" },
            { 108853, "Ribhi" },
            { 108854, "Amirah" },
            { 108855, "Joan" },
            { 108858, "Ivan" },
            { 108859, "Pat" },
            { 108860, "Elzon" },
            { 108863, "Joan" },
            { 108866, "Ivan" },
            { 108872, "Pat" },
            { 108873, "Elzon" },
            { 108876, "Amirah" },
            { 108879, "Ribhi" },
            { 108883, "Ria" },
            { 108884, "Cherrie" },
            { 108886, "Marie" },
            { 108887, "Leslee" },
            { 108888, "Joan" },
            { 108889, "Ivan" },
            { 108890, "Pat" },
            { 108904, "Elzon" },
            { 108909, "Dondi" },
            { 108912, "Dondi" },
            { 108915, "Dondi" },
            { 108945, "Dondi" },
            { 108946, "Ribhi" },
            { 108949, "Ria" },
            { 108950, "Cherrie" },
            { 108951, "Marie" },
            { 108952, "Leslee" },
            { 108953, "Joan" },
            { 108954, "Ivan" },
            { 108956, "Pat" },
            { 108957, "Elzon" },
            { 108962, "Elzon" },
            { 108963, "Ivan" },
            { 108966, "Pat" },
            { 108967, "Ribhi" },
            { 108970, "Amirah" },
            { 108971, "Ria" },
            { 108985, "Cherrie" },
            { 108986, "Marie" },
            { 108989, "Leslee" },
            { 108994, "Joan" },
            { 108999, "Dondi" },
            { 109004, "Elzon" },
            { 109006, "Ivan" },
            { 109012, "Pat" },
            { 109014, "Amirah" },
            { 109015, "Ribhi" },
            { 109017, "Ria" },
            { 109018, "Cherrie" },
            { 109029, "Marie" },
            { 109030, "Leslee" },
            { 109836, "Cherrie" },
            { 109837, "Ria" },
            { 109838, "Marie" },
            { 109839, "Leslee" },
            { 109841, "Joan" },
            { 109842, "Ribhi" },
            { 109843, "Amirah" },
            { 109844, "Ivan" },
            { 109845, "Pat" },
            { 109846, "Elzon" },
            { 109847, "Dondi" },
            { 109851, "Ribhi" },
            { 109852, "Ria" },
            { 109853, "Cherrie" },
            { 109854, "Marie" },
            { 109855, "Leslee" },
            { 109856, "Joan" },
            { 109857, "Amirah" },
            { 109858, "Dondi" },
            { 109859, "Ivan" },
            { 109860, "Pat" },
            { 109861, "Elzon" },
            { 109960, "Ribhi" },
            { 109961, "Dondi" },
            { 109962, "Ria" },
            { 109963, "Cherrie" },
            { 109964, "Marie" },
            { 109965, "Ribhi" },
            { 109966, "Joan" },
            { 109967, "Leslee" },
            { 109968, "Amirah" },
            { 109972, "Ivan" },
            { 109973, "Pat" },
            { 109974, "Elzon" },
            { 137156, "Marie" },
            { 137209, "Ria" },
            { 137218, "Cherrie" },
            { 137221, "Leslee" },
            { 137223, "Amirah" },
            { 137226, "Ivan" },
            { 137227, "Pat" },
            { 137228, "Elzon" },
            { 137232, "Ria" },
            { 137234, "Cherrie" },
            { 137275, "Leslee" },
            { 137281, "Ribhi" },
            { 137284, "Amirah" },
            { 137285, "Ivan" },
            { 137286, "Pat" },
            { 137287, "Elzon" },
            { 144636, "Dondi" },
            { 144638, "Dondi" },
            { 183453, "Dondi" },
            { 186377, "Dondi" },
            { 188388, "Dondi" },
            { 189848, "Ribhi" },
            { 190006, "Ribhi" },
            { 190009, "Ribhi" },
            { 192820, "Cherrie" },
            { 192823, "Cherrie" },
            { 192825, "Cherrie" },
            { 249061, "Amirah" },
            { 310639, "Ribhi" },
            { 327411, "Joan" },
            { 327428, "Joan" },
            { 327485, "Joan" },
            { 327534, "Joan" },
            { 407204, "Marie" },
            { 409864, "Marie" },
            { 492313, "Amirah" },
            { 494443, "Dondi" },
            { 494446, "Dondi" },
            { 494449, "Dondi" },
            { 494547, "Ribhi" },
            { 494548, "Ribhi" },
            { 494550, "Ribhi" },
            { 496249, "Pat" },
            { 496941, "Ria" },
            { 496943, "Ria" },
            { 496946, "Ria" },
            { 498384, "Pat" },
            { 505803, "Leslee" },
            { 505806, "Leslee" },
            { 526529, "Ivan" },
            { 526547, "Ivan" },
            { 526552, "Ivan" },
            { 526568, "Elzon" },
            { 1294206, "Ribhi" },
            { 1295645, "Amirah" },
            { 1304284, "Ivan" },
            { 1304558, "Elzon" },
            { 1310805, "Marie" },
            { 1319637, "Ria" },
            { 1325585, "Cherrie" },
            { 1371480, "Marie" },
            { 1378452, "Leslee" },
            { 1504777, "Pat" },
            { 1761251, "Ribhi" },
            { 1761266, "Ribhi" },
            { 1761270, "Ribhi" },
            { 1761359, "Amirah" },
            { 1761403, "Amirah" },
            { 1761453, "Amirah" },
            { 1762632, "Ivan" },
            { 1762647, "Ivan" },
            { 1762709, "Ivan" },
            { 1762713, "Elzon" },
            { 1762753, "Elzon" },
            { 1762888, "Elzon" },
            { 1763293, "Pat" },
            { 1763301, "Pat" },
            { 1763305, "Pat" },
            { 1770384, "Ria" },
            { 1770394, "Ria" },
            { 1770403, "Ria" },
            { 1790621, "Kat" },
            { 1790640, "Kat" },
            { 1790881, "Leslee" },
            { 1790895, "Leslee" },
            { 1790898, "Leslee" },
            { 1793314, "Kat" },
            { 1793340, "Kat" },
            { 1793359, "Kat" },
            { 1793372, "Kat" },
            { 1793569, "Kat" },
            { 1793579, "Kat" },
            { 1793591, "Kat" },
            { 1793617, "Kat" },
            { 1793628, "Kat" },
            { 1793675, "Kat" },
            { 1793712, "Kat" },
            { 1793782, "Kat" },
            { 1793801, "Kat" },
            { 1821655, "Cherrie" },
            { 1821667, "Cherrie" },
            { 1821672, "Cherrie" },
            { 1840035, "Marie" },
            { 1845022, "Marie" },
            { 1849882, "Marie" },
            { 1850186, "Marie" },
            { 1886868, "Ribhi" },
            { 1894466, "Ribhi" },
            { 1894487, "Ribhi" }
        };
    }
}
