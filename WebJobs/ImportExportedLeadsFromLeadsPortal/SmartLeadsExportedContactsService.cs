using Common.Database;
using Dapper;

namespace ImportExportedLeadsFromLeadsPortal;

public class SmartLeadsExportedContactsService
{
    private readonly DbConnectionFactory dbConnectionFactory;
    private readonly LeadsPortalService leadsPortalService;

    public SmartLeadsExportedContactsService(DbConnectionFactory dbConnectionFactory, LeadsPortalService leadsPortalService)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.leadsPortalService = leadsPortalService;
    }

    public async Task SaveExportedContacts(DateTime fromDate, DateTime toDate)
    {
        var exportedContacts = this.leadsPortalService.GetExportedToSmartleadsContacts(fromDate, fromDate).Result;
        if (exportedContacts == null)
        {
            Console.WriteLine("No exported contacts for {fromDate}");
            return;
        }

        Console.WriteLine($"Retrieved contacts for {fromDate}: {exportedContacts.Count()}");

        using (var connection = this.dbConnectionFactory.CreateConnection())
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    await connection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts ON;", transaction: transaction);
                    var upsert = """
                        MERGE INTO SmartLeadsExportedContacts AS Target
                        USING(
                        	VALUES (@id, @exportedDate, @email, @contactSource, @rate)
                        ) AS SOURCE (
                        	Id, ExportedDate, Email, ContactSource, Rate
                        ) ON Target.Id = source.Id
                        WHEN MATCHED THEN
                        	UPDATE SET
                        		ExportedDate = source.ExportedDate,
                        		Email = source.Email,
                        		ContactSource = source.ContactSource,
                        		Rate = source.Rate
                        WHEN NOT MATCHED THEN
                        	INSERT (Id, ExportedDate, Email, ContactSource, Rate)
                        	VALUES (@id, @exportedDate, @email, @contactSource, @rate);
                    """;
                    //var insert  = """
                    //    INSERT INTO SmartLeadsExportedContacts (Id, ExportedDate, Email, ContactSource, Rate)
                    //    VALUES (@id, @exportedDate, @email, @contactSource, @rate)
                    //""";

                    await connection.ExecuteAsync(upsert, exportedContacts, transaction);
                    await connection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts OFF;", transaction: transaction);

                    transaction.Commit();
                    Console.WriteLine($"Save {exportedContacts.Count()} contacts exported on {fromDate}");

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error saving contacts exported on {fromDate}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
