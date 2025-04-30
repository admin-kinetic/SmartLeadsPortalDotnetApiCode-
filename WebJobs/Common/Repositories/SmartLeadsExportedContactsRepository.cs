using Common.Database;
using Common.Models;
using Dapper;
using System.Text.Json;

namespace Common.Repositories
{
    public class SmartLeadsExportedContactsRepository
    {
        private readonly DbConnectionFactory dbConnectionFactory;

        public SmartLeadsExportedContactsRepository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<SmartLeadsExportedContactsBase>> GetExportedContacts()
        {
            using var connection = this.dbConnectionFactory.CreateConnection();
            const string sql = """
                SELECT TOP 500 Id, ExportedDate, Email, ContactSource, Rate
                FROM SmartLeadsExportedContacts 
                WHERE (ModifiedAt IS NULL OR ModifiedAt < DATEADD(day, -3, CAST(GETDATE() AS DATE)))
                    AND ExportedDate >= DATEADD(day, -45, CAST(GETDATE() AS DATE))
                    AND (RemovedFromSmartleads IS NULL OR RemovedFromSmartleads = 0)
                ORDER BY ExportedDate DESC;
            """;
            var result = await connection.QueryAsync<SmartLeadsExportedContactsBase>(sql);
            return result.ToList();
        }

        public async Task RemovedFromSmartlead(string email)
        {
            using var connection = this.dbConnectionFactory.CreateConnection();
            var update = """ 
                UPDATE SmartLeadsExportedContacts
                SET
                    ModifiedAt = GETDATE(),
                    RemovedFromSmartleads = 1
                WHERE Email = @email;
                """;
            await connection.ExecuteAsync(update, new { email });
        }

        public async Task UpdateHasReply(string email, List<History> history)
        {
            using var connection = this.dbConnectionFactory.CreateConnection();
            DateTime? latestRepliedAt = null;
            int? hasReply = null;
            if (history.Any(h => h.type == "REPLY"))
            {
                hasReply = 1;
                latestRepliedAt = history.Where(h => h.type == "REPLY").OrderByDescending(h => h.time).FirstOrDefault()?.time;
            }
           
            var update = """
                UPDATE SmartLeadsExportedContacts
                SET 
                    HasReply = @hasReply, 
                    ModifiedAt = GETDATE(),
                    MessageHistory = @messageHistory,
                    RepliedAt = @repliedAt
                WHERE Email = @email;
            """;

            var updateParam = new {
                hasReply,
                email,
                messageHistory = JsonSerializer.Serialize(history, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                repliedAt = latestRepliedAt
            };

            await connection.ExecuteAsync(update, updateParam);

            await this.UpdateMessageHistory(email, history);
        }

        private async Task UpdateMessageHistory(string leadEmail, List<History> history)
        {
            using var connection = this.dbConnectionFactory.CreateConnection();

            var leadEmailHistory = history.Select(h => new LeadEmailHistory
            {
                email = leadEmail,
                stats_id = h.stats_id,
                type = h.type,
                message_id = h.message_id,
                time = h.time,
                email_body = h.email_body,
                subject = h.subject,
                email_seq_number = h.email_seq_number,
                open_count = h.open_count,
                click_count = h.click_count

            });
 
            var upsert = """
                MERGE INTO MessageHistory AS target
                USING (VALUES (
                    @stats_id, @type, @message_id, @time, @email_body,
                    @subject, @email_seq_number, @open_count, @click_count, @email
                )) AS source (
                    StatsId, Type, MessageId, Time, EmailBody,
                    Subject, EmailSequenceNumber, OpenCount, ClickCount, LeadEmail
                )
                ON target.StatsId = source.StatsId
                WHEN MATCHED THEN
                    UPDATE SET
                        StatsId = source.StatsId,
                        Type = source.Type,
                        MessageId = source.MessageId,
                        Time = source.Time,
                        EmailBody = source.EmailBody,
                        Subject = source.Subject,
                        EmailSequenceNumber = source.EmailSequenceNumber,
                        OpenCount = source.OpenCount,
                        ClickCount = source.ClickCount,
                        LeadEmail = source.LeadEmail

                WHEN NOT MATCHED THEN
                    INSERT (
                        StatsId, Type, MessageId, Time, EmailBody,
                        Subject, EmailSequenceNumber, OpenCount, ClickCount, LeadEmail
                    ) VALUES (
                        @stats_id, @type, @message_id, @time, @email_body,
                        @subject, @email_seq_number, @open_count, @click_count, @email
                    );
             """;

            await connection.ExecuteAsync(upsert, leadEmailHistory);
        }
    }
}
