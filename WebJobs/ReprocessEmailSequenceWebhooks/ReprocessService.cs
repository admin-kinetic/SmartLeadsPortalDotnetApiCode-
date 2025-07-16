using Common.Database;
using Common.Models.Webhooks.Emails;
using Common.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReprocessEmailSequenceWebhooks
{
    public class ReprocessService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReprocessService> _logger;
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly MessageHistoryRepository _messageHistoryRepository;
        private readonly SmartLeadsEmailStatisticsRepository _smartLeadsEmailStatisticsRepository;

        public ReprocessService(
            IConfiguration configuration,
            ILogger<ReprocessService> logger,
            DbConnectionFactory dbConnectionFactory,
            MessageHistoryRepository messageHistoryRepository,
            SmartLeadsEmailStatisticsRepository smartLeadsEmailStatisticsRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _dbConnectionFactory = dbConnectionFactory;
            _messageHistoryRepository = messageHistoryRepository;
            _smartLeadsEmailStatisticsRepository = smartLeadsEmailStatisticsRepository;
        }

        public async Task Run()
        {
            var emailReplyWebhooks = new List<string>();

            var byEmails = _configuration.GetSection("Emails").Get<List<string>>();

            if (byEmails != null && byEmails.Any())
            {
                emailReplyWebhooks = await GetEmailReplyWebhooksByEmails(byEmails);
            }
            else
            {
                emailReplyWebhooks = await GetEmailReplyWebhooks();
            }


            foreach (var emailReplyWebhook in emailReplyWebhooks)
            {
                var emailReplyWebhookObject = default(EmailReplyPayload);
                try
                {
                    emailReplyWebhookObject = JsonSerializer.Deserialize<EmailReplyPayload>(emailReplyWebhook);
                }
                catch
                {
                    _logger.LogInformation($"Failed to deserialize webhook {emailReplyWebhook}");
                    continue;
                }

                _logger.LogInformation($"Processing emails for {emailReplyWebhookObject.to_email}, sequence number {emailReplyWebhookObject.sequence_number}");
                var emailSentWebhook = await GetFirstEmailSentWebhookByEmail(emailReplyWebhookObject.to_email, emailReplyWebhookObject.sequence_number.Value);
                if (string.IsNullOrEmpty(emailSentWebhook))
                {
                    emailSentWebhook = await GetEmailSentWebhookByEmail(emailReplyWebhookObject.to_email, emailReplyWebhookObject.sequence_number.Value);
                    if (string.IsNullOrEmpty(emailSentWebhook))
                    {
                        _logger.LogInformation($"No email sent for {emailReplyWebhookObject.to_email}, sequence number {emailReplyWebhookObject.sequence_number}");
                        continue;
                    }
                }

                _logger.LogInformation("webhook found", emailSentWebhook);

                var emailSentWebhookObject = JsonSerializer.Deserialize<EmailSentPayload>(emailSentWebhook);

                _logger.LogInformation($"Processing emails for {emailSentWebhookObject.to_email}, sequence number {emailSentWebhookObject.sequence_number}");

                await _messageHistoryRepository.UpsertEmailSent(emailSentWebhookObject);
                await _smartLeadsEmailStatisticsRepository.UpsertEmailSent(emailSentWebhookObject);

                await _messageHistoryRepository.UpsertEmailReply(emailReplyWebhookObject);
                await _smartLeadsEmailStatisticsRepository.UpdateEmailReply(emailReplyWebhookObject);
            }
        }

        private async Task<List<string>> GetEmailReplyWebhooks()
        {
            var daysOffset = _configuration.GetSection("DaysOffset").Get<int>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            var query = """
                    Select Request
                    From Webhooks 
                    Where (EventType = 'EMAIL_REPLY') AND CONVERT(DATE, CreatedAt) >= CONVERT(DATE, DATEADD(DAY, -@daysOffset, GETDATE()))
                """;
            var queryResult = await connection.QueryAsync<string>(query, new { daysOffset });
            return queryResult.ToList();
        }

        private async Task<List<string>> GetEmailReplyWebhooksByEmails(List<string> emails)
        {
            var daysOffset = _configuration.GetSection("DaysOffset").Get<int>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            var query = """
                    Select Request
                    From Webhooks 
                    Where (EventType = 'EMAIL_REPLY') 
                        AND JSON_VALUE(Request, '$.to_email') IN @emails
                """;
            var queryResult = await connection.QueryAsync<string>(query, new { emails });
            return queryResult.ToList();
        }

        private async Task<string> GetFirstEmailSentWebhookByEmail(string email, int sequenceNumber)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            var query = """
                    Select Top 1 Request
                    From Webhooks 
                    Where (EventType = 'FIRST_EMAIL_SENT') 
                        AND JSON_VALUE(Request, '$.to_email') = @email
                        AND JSON_VALUE(Request, '$.sequence_number') = @sequenceNumber
                """;
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { email, sequenceNumber });
        }


        private async Task<string> GetEmailSentWebhookByEmail(string email, int sequenceNumber)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            var query = """
                    Select Top 1 Request
                    From Webhooks 
                    Where (EventType = 'EMAIL_SENT') 
                        AND JSON_VALUE(Request, '$.to_email') = @email
                AND JSON_VALUE(Request, '$.sequence_number') = @sequenceNumber
                """;
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { email, sequenceNumber });
        }
    }
}
