using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using ZstdSharp.Unsafe;

namespace SmartLeadsDataTransfer;

public class DataTransferService
{
    private readonly string _mysqlConnectionString;
    private readonly string _sqlServerConnectionString;

    public DataTransferService(string mysqlConnectionString, string sqlServerConnectionString)
    {
        _mysqlConnectionString = mysqlConnectionString;
        _sqlServerConnectionString = sqlServerConnectionString;
    }

    // Method to transfer data from MySQL to SQL Server
    public async Task TransferData(int batchSize = 5000, int offset = 0, int limit = 25000)
    {
        using (var mySqlConnection = new MySqlConnection(_mysqlConnectionString))
        using (var sqlServerConnection = new SqlConnection(_sqlServerConnectionString))
        {
            mySqlConnection.Open();
            sqlServerConnection.Open();

            // Start a transaction in SQL Server for efficient bulk inserts
            using (var transaction = sqlServerConnection.BeginTransaction())
            {
                try
                {
                    // Enable IDENTITY_INSERT
                    await sqlServerConnection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts ON;", transaction: transaction);

                    while (true)
                    {
                        Console.WriteLine($"Fetching records from offset: {offset}");



                        // Fetch data in batches from MySQL
                        var query = $"SELECT * FROM SmartLeadsExportedContacts LIMIT {batchSize} OFFSET {offset}";
                        var records = (await mySqlConnection.QueryAsync<SmartLeadsExportedContact>(query)).ToList();

                        if (records == null || records.Count == 0)
                            break;

                        // Insert the fetched data into SQL Server in a single transaction
                        // var insertQuery = @"
                        //     INSERT INTO SmartLeadsExportedContacts (Id, ExportedDate, Email, ContactSource, Rate, HasReply, ModifiedAt,
                        //         Category, MessageHistory, LatestReplyPlainText, HasReviewed, SmartleadId, ReplyDate, RepliedAt,
                        //         FailedDelivery, RemovedFromSmartleads, SmartLeadsStatus, SmartLeadsCategory)
                        //     VALUES (@Id, @ExportedDate, @Email, @ContactSource, @Rate, @HasReply, @ModifiedAt,
                        //         @Category, @MessageHistory, @LatestReplyPlainText, @HasReviewed, @SmartleadId, @ReplyDate, @RepliedAt,
                        //         @FailedDelivery, @RemovedFromSmartleads, @SmartLeadsStatus, @SmartLeadsCategory);";

                        // await sqlServerConnection.ExecuteAsync(insertQuery, records, transaction);

                        // foreach (var record in records)
                        // {
                        //     try
                        //     {
                        //         record.RepliedAt
                        //     }
                        //     catch
                        //     {

                        //     }
                        // }

                        if (offset >= limit)
                            break;

                        offset += batchSize;
                    }

                    // var query = $"SELECT * FROM SmartLeadsExportedContacts LIMIT {batchSize} OFFSET {offset}";
                    // var records = (await mySqlConnection.QueryAsync<SmartLeadsExportedContact>(query)).ToList();

                    // if (records == null || records.Count == 0)
                    //     return;

                    // // Insert the fetched data into SQL Server in a single transaction
                    // var insertQuery = @"
                    //         INSERT INTO SmartLeadsExportedContacts (Id, ExportedDate, Email, ContactSource, Rate, HasReply, ModifiedAt,
                    //             Category, MessageHistory, LatestReplyPlainText, HasReviewed, SmartleadId, ReplyDate, RepliedAt,
                    //             FailedDelivery, RemovedFromSmartleads, SmartLeadsStatus, SmartLeadsCategory)
                    //         VALUES (@Id, @ExportedDate, @Email, @ContactSource, @Rate, @HasReply, @ModifiedAt,
                    //             @Category, @MessageHistory, @LatestReplyPlainText, @HasReviewed, @SmartleadId, @ReplyDate, @RepliedAt,
                    //             @FailedDelivery, @RemovedFromSmartleads, @SmartLeadsStatus, @SmartLeadsCategory);";

                    // await sqlServerConnection.ExecuteAsync(insertQuery, records, transaction);

                    // Enable IDENTITY_INSERT
                    await sqlServerConnection.ExecuteAsync("SET IDENTITY_INSERT SmartLeadsExportedContacts OFF;", transaction: transaction);

                    // Commit the transaction after all data has been inserted
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback in case of failure
                    transaction.Rollback();
                    throw new ApplicationException("Error during data transfer.", ex);
                }
            }
        }
    }
}
