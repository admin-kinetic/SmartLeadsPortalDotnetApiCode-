using Common.Database;
using Common.Models;
using Common.Services;
using Dapper;
using ReprocessEmailStatistics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReprocessEmailStatistics
{
    public class ReprocessEmailStatisticsService
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly SmartLeadHttpService _smartLeadHttpService;

        public ReprocessEmailStatisticsService(DbConnectionFactory dbConnectionFactory, Common.Services.SmartLeadHttpService smartLeadHttpService)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _smartLeadHttpService = smartLeadHttpService;
        }

        public async Task Run()
        {
            var emailStatisticsWithIssue = await GetEmailStatisticsWithIssue();
            if (!emailStatisticsWithIssue.Any()){
                Console.WriteLine("No email statistics with issue.");
                return;
            }

            Console.WriteLine($"Found {emailStatisticsWithIssue.Count()} email statistics with issue.");

            foreach (var emailStatistic in emailStatisticsWithIssue)
            {
                var messageHistory = await _smartLeadHttpService.MessageHistoryByLead(emailStatistic.CampaignId, emailStatistic.LeadId, emailStatistic.ApiKey);
                var sentMessages = messageHistory.history.Where(x => x.type == "SENT");
                foreach (var sentMessage in sentMessages)
                {
                    await this.UpsertEmailSent(emailStatistic.LeadId, emailStatistic.LeadName, emailStatistic.LeadEmail, sentMessage);
                }
            }
        }

        private async Task<List<EmailStatisticsWithIssue>> GetEmailStatisticsWithIssue()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var query = """
                Select sla.Id as [AccountId], sla.Name as [AccountName], sla.ApiKey, slc.Id as [CampaignId], sles.LeadId, sles.LeadEmail, sles.SequenceNumber, sles.LeadName
                From SmartLeadsEmailStatistics sles
                Inner Join SmartLeadAllLeads slal On slal.LeadId = sles.LeadId
                Inner Join SmartLeadCampaigns slc On slc.Id = slal.CampaignId
                Inner Join SmartleadsAccountCampaigns slsc ON slsc.CampaignId = slal.CampaignId
                Inner Join SmartleadsAccounts sla ON sla.Id = slsc.SmartleadsAccountId
                Where SentTime IS NULL 
                Order By CreatedAt ASC
            """;

            var result = await connection.QueryAsync<EmailStatisticsWithIssue>(query);
            return result.ToList();
        }

        internal async Task UpsertEmailSent(string leadId, string leadName, string toEmail, History history)
        {
            Console.WriteLine($"UpsertEmailSent for {toEmail}");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            using var connection = _dbConnectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var upsert = """
                MERGE INTO SmartLeadsEmailStatistics WITH (ROWLOCK) AS target
                USING ( 
                    VALUES (@leadEmail, @sequenceNumber)
                ) AS source (LeadEmail, SequenceNumber)
                ON target.LeadEmail = source.LeadEmail AND target.SequenceNumber = source.SequenceNumber
                WHEN MATCHED THEN
                    UPDATE SET 
                        LeadId = @leadId,
                        LeadName = @leadName,
                        EmailSubject = @emailSubject,
                        EmailMessage = @emailMessage,
                        SentTime = @sentTime
                WHEN NOT MATCHED THEN
                    INSERT (Guid, LeadId, LeadEmail, LeadName, SequenceNumber, EmailSubject, EmailMessage, SentTime)
                        VALUES (NewId(), @leadId, @leadEmail, @leadName, @sequenceNumber, @emailSubject, @emailMessage, @sentTime);
            """;

                await connection.ExecuteAsync(upsert,
                    new
                    {
                        leadId = leadId,
                        leadEmail = toEmail,
                        leadName = leadName,
                        sequenceNumber = history.email_seq_number,
                        emailSubject = history.subject,
                        emailMessage = history.email_body,
                        sentTime = history.time
                    },
                    transaction);
                await transaction.CommitAsync();
                Console.WriteLine($"UpsertEmailSent took {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Error on UpsertEmailSent", ex);
                throw;
            }
        }
    }
}
