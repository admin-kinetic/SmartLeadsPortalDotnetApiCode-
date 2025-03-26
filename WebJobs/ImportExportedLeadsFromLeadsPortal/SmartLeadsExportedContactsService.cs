using System;
using System.Threading.Tasks;
using Dapper;

namespace ExportedLeadsFromLeadsPortal;

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
        var exportedContacts = this.leadsPortalService.GetExportedToSmartleadsContacts(fromDate, toDate).Result;

        using (var connection = this.dbConnectionFactory.CreateConnection())
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    await connection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts ON;", transaction: transaction);

                    var insert  = """
                        INSERT INTO SmartLeadsExportedContacts (Id, ExportedDate, Email, ContactSource, Rate)
                        VALUES (@id, @exportedDate, @email, @contactSource, @rate)
                    """;

                    await connection.ExecuteAsync(insert, exportedContacts, transaction);
                    await connection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts OFF;", transaction: transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
