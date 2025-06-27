using Dapper;
using SmartLeadsPortalDotNetApi.Database;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Model.Webhooks.Emails;

namespace SmartLeadsPortalDotNetApi.Repositories
{
    public class MessageHistoryRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<MessageHistoryRepository> logger;

        public MessageHistoryRepository(
            DbConnectionFactory dbConnectionFactory, 
            ILogger<MessageHistoryRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            this.logger = logger;
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
            this.logger.LogInformation($"Start UpsertEmailSent {emailOpenPayload.to_email}");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

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

                // First check if the record exists
                var existingRecord = await connection.QueryFirstOrDefaultAsync<int>("""
                    SELECT 1 FROM MessageHistory 
                    WHERE MessageId = @message_id
                """, new { message_id = email.message_id }, transaction);

                if (existingRecord != 0)
                {
                    // Update existing record
                    await connection.ExecuteAsync("""
                        UPDATE MessageHistory 
                        SET StatsId = @stats_id,
                            Type = @type,
                            Time = @time,
                            EmailBody = @email_body,
                            Subject = @subject,
                            EmailSequenceNumber = @email_seq_number,
                            LeadEmail = @email
                        WHERE MessageId = @message_id
                    """, email, transaction);
                }
                else
                {
                    // Insert new record
                    await connection.ExecuteAsync("""
                        INSERT INTO MessageHistory (
                            StatsId, Type, MessageId, Time, EmailBody,
                            Subject, EmailSequenceNumber, LeadEmail
                        ) VALUES (
                            @stats_id, @type, @message_id, @time, @email_body,
                            @subject, @email_seq_number, @email
                        )
                    """, email, transaction);
                }

                await transaction.CommitAsync();
                this.logger.LogInformation($"Successfully UpsertEmailSent for {emailOpenPayload.to_email}, took {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                this.logger.LogError($"Error on UpsertEmailSent for {emailOpenPayload.to_email}", ex.Message);
                throw;
            }
        }

        internal async Task UpsertEmailReply(EmailReplyPayload payloadObject)
        {
            this.logger.LogInformation($"Start UpsertEmailReply {payloadObject.to_email}");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await using var connection = await _dbConnectionFactory.GetSqlConnectionAsync();

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
                    stats_id = payloadObject.stats_id,
                    type = "REPLY",
                    message_id = payloadObject.reply_message?.message_id, // Added null check
                    time = payloadObject.event_timestamp,
                    email_body = payloadObject.reply_body,
                    subject = payloadObject.subject,
                    email_seq_number = payloadObject.sequence_number
                };

                // First check if the record exists
                var existingRecord = await connection.QueryFirstOrDefaultAsync<int>("""
                    SELECT 1 FROM MessageHistory 
                    WHERE MessageId = @message_id
                """, new { message_id = email.message_id }, transaction);

                if (existingRecord != 0)
                {
                    // Update existing record
                    await connection.ExecuteAsync("""
                        UPDATE MessageHistory 
                        SET StatsId = @stats_id,
                            Type = @type,
                            Time = @time,
                            EmailBody = @email_body,
                            Subject = @subject,
                            EmailSequenceNumber = @email_seq_number,
                            LeadEmail = @email
                        WHERE MessageId = @message_id
                    """, email, transaction);
                }
                else
                {
                    // Insert new record
                    await connection.ExecuteAsync("""
                        INSERT INTO MessageHistory (
                            StatsId, Type, MessageId, Time, EmailBody,
                            Subject, EmailSequenceNumber, LeadEmail
                        ) VALUES (
                            @stats_id, @type, @message_id, @time, @email_body,
                            @subject, @email_seq_number, @email
                        )
                    """, email, transaction);
                }

                await transaction.CommitAsync();
                this.logger.LogInformation($"Successfully UpsertEmailReply for {payloadObject.to_email}, took {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                this.logger.LogError($"Error on UpsertEmailReply for {payloadObject.to_email}", ex.Message);
                throw;
            }
        }
    }
}
