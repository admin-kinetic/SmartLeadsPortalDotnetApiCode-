using Common.Database;
using Common.Entities;
using Common.Models.Webhooks.Emails;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Common.Repositories
{
    public class MessageHistoryRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly ILogger logger;

        public MessageHistoryRepository(
            DbConnectionFactory dbConnectionFactory, 
            ILogger logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            this.logger = logger;
        }

        public async Task<List<MessageHistory>> GetByEmail(string email)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
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

        public async Task UpsertEmailSent(EmailSentPayload emailOpenPayload)
        {
            logger.LogInformation($"Start UpsertEmailSent {emailOpenPayload.to_email}");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            using var connection = _dbConnectionFactory.CreateConnection();

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
                    emailOpenPayload.stats_id,
                    type = "SENT",
                    emailOpenPayload.message_id,
                    time = emailOpenPayload.time_sent,
                    email_body = emailOpenPayload.sent_message_body,
                    emailOpenPayload.subject,
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
                    ON target.MessageId = source.MessageId
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
                logger.LogInformation($"Successfully UpsertEmailSent for {emailOpenPayload.to_email}, took {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError($"Error on UpsertEmailSent for {emailOpenPayload.to_email}", ex.Message);
                throw;
            }
        }

        public async Task UpsertEmailReply(EmailReplyPayload payloadObject)
        {
            logger.LogInformation($"Start UpsertEmailReply {payloadObject.to_email}");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            using var connection = _dbConnectionFactory.CreateConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var email = new
                {
                    email = payloadObject.sl_lead_email,
                    payloadObject.stats_id,
                    type = "REPLY",
                    payloadObject.reply_message.message_id,
                    time = payloadObject.event_timestamp,
                    email_body = payloadObject.reply_body,
                    payloadObject.subject,
                    email_seq_number = payloadObject.sequence_number
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
                    ON target.MessageId = source.MessageId
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
                logger.LogInformation($"Successfully UpsertEmailSent for {payloadObject.to_email}, took {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError($"Error on UpsertEmailSent for {payloadObject.to_email}", ex.Message);
                throw;
            }
           
        }
    }
}
