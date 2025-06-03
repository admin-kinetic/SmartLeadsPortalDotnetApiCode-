using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class MessageHistoryRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public MessageHistoryRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<MessageHistory>> GetByEmail(string email)
        {
            using var connection = _dbConnectionFactory.GetSqlConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var query = """
                SELECT * FROM MessageHistory 
                Where LeadEmail = @email
                ORDER BY Time DESC  
            """;

            var queryResult = await connection.QueryAsync<MessageHistory>(query, new { email });
            return queryResult.ToList();
        }

        internal async Task UpsertEmailSent(EmailSentPayload emailOpenPayload)
        {
            using var connection = _dbConnectionFactory.GetSqlConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var email = new
                {
                    email = emailOpenPayload.to_email,
                    stats_id = emailOpenPayload.stats_id,
                    type = "SENT",
                    message_id = emailOpenPayload.message_id,
                    time = emailOpenPayload.time_sent,
                    email_body = emailOpenPayload.sent_message_body,
                    subject = emailOpenPayload.subject,
                    email_seq_number = emailOpenPayload.sequence_number
                };

                var upsert = """
                MERGE INTO MessageHistory WITH (ROWLOCK) AS target
                USING (VALUES (
                    @stats_id, @type, @message_id, @time, @email_body,
                    @subject, @email_seq_number, @email
                )) AS source (
                    StatsId, Type, MessageId, Time, EmailBody,
                    Subject, EmailSequenceNumber, LeadEmail
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
                        LeadEmail = source.LeadEmail

                WHEN NOT MATCHED THEN
                    INSERT (
                        StatsId, Type, MessageId, Time, EmailBody,
                        Subject, EmailSequenceNumber, LeadEmail
                    ) VALUES (
                        @stats_id, @type, @message_id, @time, @email_body,
                        @subject, @email_seq_number, @email
                    );
             """;

                await connection.ExecuteAsync(upsert, email, transaction);
                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        internal async Task UpsertEmailReply(EmailReplyPayload payloadObject)
        {
            using var connection = _dbConnectionFactory.GetSqlConnection();

            var email = new
            {
                email = payloadObject.sl_lead_email,
                stats_id = payloadObject.stats_id,
                type = "REPLY",
                message_id = payloadObject.message_id,
                time = payloadObject.event_timestamp,
                email_body = payloadObject.sent_message_body,
                subject = payloadObject.subject,
                email_seq_number = payloadObject.sequence_number
            };

            var upsert = """
                MERGE INTO MessageHistory AS target
                USING (VALUES (
                    @stats_id, @type, @message_id, @time, @email_body,
                    @subject, @email_seq_number, @email
                )) AS source (
                    StatsId, Type, MessageId, Time, EmailBody,
                    Subject, EmailSequenceNumber, LeadEmail
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
                        LeadEmail = source.LeadEmail

                WHEN NOT MATCHED THEN
                    INSERT (
                        StatsId, Type, MessageId, Time, EmailBody,
                        Subject, EmailSequenceNumber, LeadEmail
                    ) VALUES (
                        @stats_id, @type, @message_id, @time, @email_body,
                        @subject, @email_seq_number, @email
                    );
             """;

            await connection.ExecuteAsync(upsert, email);
        }
    }
}
